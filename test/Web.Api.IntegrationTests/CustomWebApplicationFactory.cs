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
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Web.Api.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTests");
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

                            // Add a database context (AppDbContext) using an in-memory database for testing.
                            services.AddDbContextPool<AppDbContext>(options =>
                            {
                                options.UseInMemoryDatabase("InMemoryAppDb");
                                options.UseInternalServiceProvider(serviceProvider);
                            });

                            services.AddDbContextPool<AppIdentityDbContext>(options =>
                            {
                                options.UseInMemoryDatabase("InMemoryIdentityDb");
                                options.UseInternalServiceProvider(serviceProvider);
                            });
                            services.AddScoped<SignInManager<AppUser>>();
                            services.AddScoped<ILogger<UserRepository>>(provider => {
                                ILoggerFactory loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                                return loggerFactory.CreateLogger<UserRepository>();
                            });
                            services.AddDistributedMemoryCache();
                            // Build the service provider.
                            var sp = services.BuildServiceProvider();

                            // Create a scope to obtain a reference to the database contexts
                            using (var scope = sp.CreateScope())
                            {
                                var scopedServices = scope.ServiceProvider;
                                var appDb = scopedServices.GetRequiredService<AppDbContext>();
                                var identityDb = scopedServices.GetRequiredService<AppIdentityDbContext>();
                                var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

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
    }
}