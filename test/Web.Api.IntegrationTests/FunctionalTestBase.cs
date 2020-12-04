using System;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
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
using Polly;
using Polly.Extensions.Http;
using Web.Api;

namespace Web.Api.IntegrationTests
{
    public class FunctionalTestBase : IDisposable
    {
        IConfigurationRoot _config;
        private GrpcChannel? _channel;
        protected GrpcTestFixture<Startup> Fixture { get; init; } = default!;
        protected ILoggerFactory LoggerFactory => Fixture.LoggerFactory;
        protected GrpcChannel Channel => _channel ??= CreateChannel();
        public FunctionalTestBase()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTests");
            string contentRootFull = Path.GetFullPath(Directory.GetCurrentDirectory());
            _config = new ConfigurationBuilder()
                .SetBasePath(contentRootFull)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.Development.json", true, true)
                .AddEnvironmentVariables().Build();
            Fixture = new GrpcTestFixture<Startup>(ConfigureServices);
        }
        protected GrpcChannel CreateChannel()
        {
            return GrpcChannel.ForAddress(Fixture.Client.BaseAddress, new GrpcChannelOptions
            {
                LoggerFactory = LoggerFactory,
                HttpClient = Fixture.Client
            });
        }
        protected virtual void ConfigureServices(IServiceCollection services)
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase().AddLogging()
                .BuildServiceProvider();
                //.EnableCallContextPropagation();
                //.AddInterceptor(() => new LoggingInterceptor());
            // Add a database context (Biz4xDbContext) using an in-memory database for testing.
            services.AddDbContextPool<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("GrpcInMemoryAppDb");
                    options.UseInternalServiceProvider(serviceProvider);
                });
            services.AddDbContextPool<AppIdentityDbContext>(options =>
                {
                    options.UseInMemoryDatabase("GrpcInMemoryIdentityDb");
                    options.UseInternalServiceProvider(serviceProvider);
                });
            services.AddScoped<SignInManager<AppUser>>();
            services.AddScoped<ILogger<UserRepository>>(provider => {
                ILoggerFactory loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                return loggerFactory.CreateLogger<UserRepository>();
            });
            services.AddDistributedMemoryCache();
            services.AddOptions();
            services.Configure<GrpcConfig>(_config.GetSection(nameof(GrpcConfig)));
            #if false
            //Only add this for debugging purpose. Very verbose otherwise.
            services.AddLogging(config =>
                {
                    // clear out default configuration
                    config.ClearProviders();
                    config.AddConfiguration(_config.GetSection("Logging"));
                    config.AddDebug();
                    config.AddEventSourceLogger();
                    config.AddConsole();
                });
            #endif
            // Build the service provider.
            IServiceProvider ServiceProvider = services.BuildServiceProvider();
            // Create a scope to obtain a reference to the database contexts
            using (var scope = ServiceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var appDb = scopedServices.GetRequiredService<AppDbContext>();
                var identityDb = scopedServices.GetRequiredService<AppIdentityDbContext>();
                var logger = scopedServices.GetRequiredService<ILogger<FunctionalTestBase>>();
                // Ensure the database is created.
                appDb.Database.EnsureCreated();
                identityDb.Database.EnsureCreated();
                try
                {
                    // Seed the database with test data.
                    SeedData.PopulateTestData(identityDb);
                    SeedData.PopulateTestData(appDb);
                } catch (Exception ex)
                {
                    logger.LogError(ex, $"An error occurred seeding the database with test messages. Error: {ex.Message}");
                }
            }
        }
        public void Dispose()
        {
            Fixture.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}