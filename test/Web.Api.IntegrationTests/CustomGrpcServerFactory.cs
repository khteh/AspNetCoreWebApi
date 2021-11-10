﻿using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Web.Api.Infrastructure.Data;
using Web.Api.Infrastructure.Identity;
using Web.Api.Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.TestHost;
using Grpc.Net.ClientFactory;
using Grpc.Net.ClientFactory.Internal;
using Web.Api.Identity.Accounts;
using Web.Api.Identity.Auth;
using Web.Api.Core.Configuration;
using Microsoft.Extensions.Configuration;
using System.IO;
using Web.Api.IntegrationTests.Services;
using Web.Api.Identity;
using static Web.Api.Identity.Accounts.Accounts;
using static Web.Api.Identity.Auth.Auth;
using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using Grpc.Net.Client;
using Polly;
using Polly.Extensions.Http;

namespace Web.Api.IntegrationTests
{
    public class CustomGrpcServerFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public ServiceProvider ServiceProvider {get; private set;}
        protected override IHostBuilder CreateHostBuilder()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTests");
            string contentRootFull = Path.GetFullPath(Directory.GetCurrentDirectory());
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(contentRootFull)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.Development.json", true, true)
                .AddEnvironmentVariables().Build();
            GrpcConfig grpcConfig = config.GetSection(nameof(GrpcConfig)).Get<GrpcConfig>();
            return Host.CreateDefaultBuilder()
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        // use whatever config you want here
                        webBuilder.UseStartup<Program>()
                            .UseTestServer()
                            .ConfigureServices(services =>
                            {
                                // Create a new service provider.
                                var serviceProvider = new ServiceCollection()
                                    .AddEntityFrameworkInMemoryDatabase().AddLogging()
                                    .BuildServiceProvider();
                                AddGrpcClient<AccountsClient>(services, new Uri(grpcConfig.Endpoint));
                                    //.AddInterceptor(() => new LoggingInterceptor());
                                    //.AddHttpMessageHandler(() => ClientTestHelpers.CreateTestMessageHandler(new HelloReply()));
                                AddGrpcClient<AuthClient>(services, new Uri(grpcConfig.Endpoint));
                                    //.EnableCallContextPropagation(); Only use this in gRPC service, not client.
                                    //.AddInterceptor(() => new LoggingInterceptor());

                                // Add a database context (AppDbContext) using an in-memory database for testing.
                                services.AddDbContextPool<AppDbContext>(options =>
                                {
                                    options.UseInMemoryDatabase("GrpcInMemoryAppDb");
                                    options.UseInternalServiceProvider(serviceProvider);
                                    options.EnableSensitiveDataLogging();
                                    options.EnableDetailedErrors();
                                    options.LogTo(Console.WriteLine);
                                });

                                services.AddDbContextPool<AppIdentityDbContext>(options =>
                                {
                                    options.UseInMemoryDatabase("GrpcInMemoryIdentityDb");
                                    options.UseInternalServiceProvider(serviceProvider);
                                    options.EnableSensitiveDataLogging();
                                    options.EnableDetailedErrors();
                                    options.LogTo(Console.WriteLine);
                                });
                                services.AddScoped<SignInManager<AppUser>>();
                                services.AddScoped<ILogger<UserRepository>>(provider => {
                                    ILoggerFactory loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                                    return loggerFactory.CreateLogger<UserRepository>();
                                });
                                services.AddDistributedMemoryCache();
                                services.AddOptions();
                                services.Configure<GrpcConfig>(config.GetSection(nameof(GrpcConfig)));
                                // Build the service provider.
                                ServiceProvider = services.BuildServiceProvider();
                                // Create a scope to obtain a reference to the database contexts
                                using (var scope = ServiceProvider.CreateScope())
                                {
                                    var scopedServices = scope.ServiceProvider;
                                    var appDb = scopedServices.GetRequiredService<AppDbContext>();
                                    var identityDb = scopedServices.GetRequiredService<AppIdentityDbContext>();
                                    var logger = scopedServices.GetRequiredService<ILogger<CustomGrpcServerFactory<TStartup>>>();

                                    // Ensure the database is created.
                                    appDb.Database.EnsureCreated();
                                    identityDb.Database.EnsureCreated();

                                    try
                                    {
                                        // Seed the database with test data.
                                        SeedData.PopulateTestData(identityDb);
                                        SeedData.PopulateTestData(appDb);
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.LogError(ex, $"An error occurred seeding the " +
                                            $"database with test messages. Error: {ex.Message}");
                                    }
                                }
                            });
                    });
        }
        public IHttpClientBuilder AddGrpcClient<TClient>(IServiceCollection services, Uri uri) where TClient : global::Grpc.Core.ClientBase
            //HttpMessageHandler handler = Server.CreateHandler();
            => services.AddGrpcClient<TClient>(o => {o.Address = uri;})
                .ConfigurePrimaryHttpMessageHandler(() => Server.CreateHandler())
                .AddPolicyHandler(GetRetryPolicy());
        private IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() =>
            HttpPolicyExtensions.HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(new Random().Next(0, 100)));
    }
}