using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web.Api.Core.Configuration;
using Web.Api.Infrastructure.Data;
using Web.Api.Infrastructure.Data.Repositories;
using Web.Api.Infrastructure.Identity;
namespace Web.Api.IntegrationTests;
public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    private IServiceCollection _services;
    private ServiceProvider _sp;
    private CustomWebApplicationFactory() => Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTests");
    #if false
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTests");
        builder.ConfigureServices((context, services) =>
        {
            // Create a new service provider.
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextPool<AppDbContext>));
            services.Remove(descriptor);
            descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextPool<AppIdentityDbContext>));
            services.Remove(descriptor);
            services.AddDbContextPool<AppIdentityDbContext>(options =>
            {
                options.UseNpgsql(context.Configuration.GetConnectionString("IntegrationTests"), b => b.MigrationsAssembly("Web.Api.Infrastructure"));
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                options.LogTo(Console.WriteLine);
            })
                .AddDbContextPool<AppDbContext>(options =>
                {
                    options.UseNpgsql(context.Configuration.GetConnectionString("IntegrationTests"), b => b.MigrationsAssembly("Web.Api.Infrastructure"));
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                    options.LogTo(Console.WriteLine);
                });
            services.AddLogging();
            services.AddOptions();
            services.Configure<GrpcConfig>(context.Configuration.GetSection(nameof(GrpcConfig)));
            services.AddScoped<SignInManager<AppUser>>();
            services.AddScoped<ILogger<UserRepository>>(provider =>
            {
                ILoggerFactory loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                return loggerFactory.CreateLogger<UserRepository>();
            });
            services.AddDistributedMemoryCache();
            _services = services;
            // Build the service provider.
            _sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database contexts
            using (var scope = _sp.CreateScope())
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
                    SeedData.PopulateTestData(identityDb, appDb);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"An error occurred seeding the database with test messages. Error: {ex.Message}");
                }
            }
        });
    }
    #endif
    public static CustomWebApplicationFactory<TStartup> CreateWebApplicationFactory()
    {
        var factory = new CustomWebApplicationFactory<TStartup>();
        
        // Create a scope to obtain a reference to the database contexts
        using (var scope = factory.Services.CreateScope())
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
                SeedData.PopulateTestData(identityDb, appDb);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred seeding the database with test messages. Error: {ex.Message}");
                throw;
            }
        }
        return factory;
    }
    public void Dispose()
    {
        //var sp = _services.BuildServiceProvider();
        // Create a scope to obtain a reference to the database contexts
        //using (var scope = _sp.CreateScope())
        using (var scope = Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var appDb = scopedServices.GetRequiredService<AppDbContext>();
            var identityDb = scopedServices.GetRequiredService<AppIdentityDbContext>();
            SeedData.CleanUpTestData(identityDb, appDb);
        }
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}