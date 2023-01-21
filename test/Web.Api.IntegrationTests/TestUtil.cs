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
using Web.Api.Core.Configuration;
using Web.Api.Infrastructure.Data;
using Web.Api.Infrastructure.Data.Repositories;
using Web.Api.Infrastructure.Identity;
namespace Web.Api.IntegrationTests;
public class TestUtil
{
    #if false
    public static async Task<CustomWebApplicationFactory<TStartup>> CreateWebApplicationFactory<TStartup>()
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
                await SeedData.PopulateTestDataAsync(identityDb, appDb);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred seeding the database with test messages. Error: {ex.Message}");
                throw;
            }
        }
        return factory;
    }
    #endif
}