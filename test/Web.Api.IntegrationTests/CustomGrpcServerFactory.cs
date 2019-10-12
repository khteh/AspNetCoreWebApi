using System;
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
using Web.Api.Core.Accounts;
using Web.Api.Core.Auth;
using Web.Api.Core.Configuration;
using Microsoft.Extensions.Configuration;
using System.IO;
using Web.Api.IntegrationTests.Services;
using Web.Api.Core.Grpc;
using static Web.Api.Core.Accounts.Accounts;
using static Web.Api.Core.Auth.Auth;
using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using Grpc.Net.Client;
using Polly;
using Polly.Extensions.Http;

namespace Web.Api.IntegrationTests
{
    public class CustomGrpcServerFactory<TStartup> : WebApplicationFactory<Startup>
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
                        webBuilder.UseStartup<Startup>()
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
                                    //.EnableCallContextPropagation();
                                    //.AddInterceptor(() => new LoggingInterceptor());

                                // Add a database context (AppDbContext) using an in-memory database for testing.
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
                                services.Configure<GrpcConfig>(config.GetSection(nameof(GrpcConfig)));
                                #if false
                                services.AddTransient<IAccountsGrpcClient<RegisterUserRequest, RegisterUserResponse>, AccountsGrpcClient<RegisterUserRequest, RegisterUserResponse>>();
                                services.AddTransient<IAccountsGrpcClient<ChangePasswordRequest, Response>, AccountsGrpcClient<ChangePasswordRequest, Response>>();
                                services.AddTransient<IAccountsGrpcClient<ResetPasswordRequest, Response>, AccountsGrpcClient<ResetPasswordRequest, Response>>();
                                services.AddTransient<IAccountsGrpcClient<StringInputParameter, Response>, AccountsGrpcClient<StringInputParameter, Response>>();
                                services.AddTransient<IAccountsGrpcClient<StringInputParameter, DeleteUserResponse>, AccountsGrpcClient<StringInputParameter, DeleteUserResponse>>();
                                services.AddTransient<IAccountsGrpcClient<StringInputParameter, FindUserResponse>, AccountsGrpcClient<StringInputParameter, FindUserResponse>>();
                                services.AddHttpsClient<IAccountsGrpcClient<RegisterUserRequest, RegisterUserResponse>, AccountsGrpcClient<RegisterUserRequest, RegisterUserResponse>>();
                                services.AddHttpsClient<IAccountsGrpcClient<ChangePasswordRequest, Response>, AccountsGrpcClient<ChangePasswordRequest, Response>>();
                                services.AddHttpsClient<IAccountsGrpcClient<ResetPasswordRequest, Response>, AccountsGrpcClient<ResetPasswordRequest, Response>>();
                                services.AddHttpsClient<IAccountsGrpcClient<StringInputParameter, Response>, AccountsGrpcClient<StringInputParameter, Response>>();
                                services.AddHttpsClient<IAccountsGrpcClient<StringInputParameter, DeleteUserResponse>, AccountsGrpcClient<StringInputParameter, DeleteUserResponse>>();
                                services.AddHttpsClient<IAccountsGrpcClient<StringInputParameter, FindUserResponse>, AccountsGrpcClient<StringInputParameter, FindUserResponse>>();

                                services.AddTransient<IAccountsGrpcClient<LoginRequest, LoginResponse>, AccountsGrpcClient<LoginRequest, LoginResponse>>();
                                services.AddTransient<IAccountsGrpcClient<ExchangeRefreshTokenResponse, ExchangeRefreshTokenRequest>, AccountsGrpcClient<ExchangeRefreshTokenResponse, ExchangeRefreshTokenRequest>>();
                                services.AddHttpsClient<IAccountsGrpcClient<LoginRequest, LoginResponse>, AccountsGrpcClient<LoginRequest, LoginResponse>>();
                                services.AddHttpsClient<IAccountsGrpcClient<ExchangeRefreshTokenResponse, ExchangeRefreshTokenRequest>, AccountsGrpcClient<ExchangeRefreshTokenResponse, ExchangeRefreshTokenRequest>>();
                                #endif
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
        {
            //HttpMessageHandler handler = Server.CreateHandler();
            return services.AddGrpcClient<TClient>(o => {o.Address = uri;})
                .ConfigurePrimaryHttpMessageHandler(() => Server.CreateHandler())
                .AddPolicyHandler(GetRetryPolicy());
        }
        private IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() =>
            HttpPolicyExtensions.HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(new Random().Next(0, 100)));

    }
}