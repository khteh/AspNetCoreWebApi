﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Web.Api.Core.Configuration;
using Web.Api.Infrastructure.Data;
using Web.Api.Infrastructure.Identity;
using Xunit;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Web.Api.IntegrationTests;
public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>, IAsyncLifetime where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Route the application's logs to the xunit output
        builder.UseEnvironment("IntegrationTests");
        builder.ConfigureLogging((p) => p.SetMinimumLevel(LogLevel.Debug));
        builder.ConfigureServices((context, services) =>
        {
            // Create a new service provider.
            services.Configure<GrpcConfig>(context.Configuration.GetSection(nameof(GrpcConfig)));
            services.AddScoped<SignInManager<AppUser>>();
        });
    }
    public async ValueTask InitializeAsync()
    {
        using (var scope = Services.CreateScope())
            try
            {
                var scopedServices = scope.ServiceProvider;
                var appDb = scopedServices.GetRequiredService<AppDbContext>();
                var identityDb = scopedServices.GetRequiredService<AppIdentityDbContext>();
                ILoggerFactory loggerFactory = scopedServices.GetRequiredService<ILoggerFactory>();
                ILogger logger = loggerFactory.CreateLogger<CustomWebApplicationFactory<TStartup>>();
                // Ensure the database is created.
                await appDb.Database.EnsureCreatedAsync();
                await identityDb.Database.EnsureCreatedAsync();
                // Seed the database with test data.
                logger.LogDebug($"{nameof(InitializeAsync)} populate test data...");
                await SeedData.PopulateTestData(identityDb, appDb);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{nameof(InitializeAsync)} exception! {ex}");
                throw;
            }
    }
    public async override ValueTask DisposeAsync()
    {
        //var sp = _services.BuildServiceProvider();
        // Create a scope to obtain a reference to the database contexts
        //using (var scope = _sp.CreateScope())
        using (var scope = Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var appDb = scopedServices.GetRequiredService<AppDbContext>();
            var identityDb = scopedServices.GetRequiredService<AppIdentityDbContext>();
            await SeedData.CleanUpTestData(identityDb, appDb);
        }
        await base.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}