using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Web.Api.Core.Configuration;
using Web.Api.Infrastructure.Data;
using Web.Api.Infrastructure.Data.Repositories;
using Web.Api.Infrastructure.Identity;

namespace Web.Api.IntegrationTests
{
    public class CustomGRPCWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private IConfigurationRoot _config;
        public CustomGRPCWebApplicationFactory(IConfigurationRoot config) => _config = config;
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
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
                services.AddScoped<ILogger<UserRepository>>(provider =>
                {
                    ILoggerFactory loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                    return loggerFactory.CreateLogger<UserRepository>();
                });
                services.AddDistributedMemoryCache();
                services.AddOptions();
                services.Configure<GrpcConfig>(_config.GetSection(nameof(GrpcConfig)));
                // Build the service provider.
                IServiceProvider ServiceProvider = services.BuildServiceProvider();
                // Create a scope to obtain a reference to the database contexts
                using (var scope = ServiceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var appDb = scopedServices.GetRequiredService<AppDbContext>();
                    var identityDb = scopedServices.GetRequiredService<AppIdentityDbContext>();
                    var logger = scopedServices.GetRequiredService<ILogger<GrpcTestFixture<Program>>>();
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
                        logger.LogError(ex, $"An error occurred seeding the database with test messages. Error: {ex.Message}");
                    }
                }
            });
        }
    }
}
