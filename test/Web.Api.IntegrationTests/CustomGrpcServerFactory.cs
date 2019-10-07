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

namespace Web.Api.IntegrationTests
{
    public class CustomGrpcServerFactory<TStartup> : WebApplicationFactory<Startup>
    {
        public ServiceProvider ServiceProvider {get; private set;}
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTests");
            var contentRootFull = Path.GetFullPath(Directory.GetCurrentDirectory());
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(contentRootFull)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.Development.json", true, true)
                .AddJsonFile($"appsettings.local.json", true, true)
                .AddEnvironmentVariables().Build();
            builder.ConfigureServices(services =>
            {
                // Create a new service provider.
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase().AddLogging()
                    .BuildServiceProvider();
                services.AddGrpcClient<Accounts.Accounts.AccountsClient>(o => { o.Address = new Uri("http://localhost");})
                    .EnableCallContextPropagation();
                    //.AddInterceptor(() => new LoggingInterceptor());
                    //.AddHttpMessageHandler(() => ClientTestHelpers.CreateTestMessageHandler(new HelloReply()));
                services.AddGrpcClient<Auth.Auth.AuthClient>(o => { o.Address = new Uri("http://localhost");})
                    .EnableCallContextPropagation();
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
                services.AddTransient<IGrpcClient<Accounts.RegisterUserRequest, Accounts.RegisterUserResponse>, AccountsClient<Accounts.RegisterUserRequest, Accounts.RegisterUserResponse>>();
                services.AddTransient<IGrpcClient<Accounts.ChangePasswordRequest, Grpc.Response>, AccountsClient<Accounts.ChangePasswordRequest, Grpc.Response>>();
                services.AddTransient<IGrpcClient<Accounts.ResetPasswordRequest, Grpc.Response>, AccountsClient<Accounts.ResetPasswordRequest, Grpc.Response>>();
                services.AddTransient<IGrpcClient<Accounts.ResetPasswordRequest, Grpc.Response>, AccountsClient<Accounts.ResetPasswordRequest, Grpc.Response>>();

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
                        logger.LogError(ex, "An error occurred seeding the " +
                            "database with test messages. Error: {ex.Message}");
                    }
                }
            });
        }
    }
}