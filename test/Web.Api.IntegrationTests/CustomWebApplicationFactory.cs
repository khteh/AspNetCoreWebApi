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
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;

namespace Web.Api.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTests");
            builder.ConfigureServices(services => {
                // Create a new service provider.
#if false
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase().AddLogging()
                    .BuildServiceProvider();
#endif
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextPool<AppDbContext>));
                services.Remove(descriptor);
                descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextPool<AppIdentityDbContext>));
                services.Remove(descriptor);

                services.AddEntityFrameworkInMemoryDatabase().AddLogging();

                // Add a database context (AppDbContext) using an in-memory database for testing.
                services.AddDbContextPool<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryAppDb");
                    //options.UseInternalServiceProvider(serviceProvider);
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                    options.LogTo(Console.WriteLine);
                });

                services.AddDbContextPool<AppIdentityDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryIdentityDb");
                    //options.UseInternalServiceProvider(serviceProvider);
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
        }
    }
}