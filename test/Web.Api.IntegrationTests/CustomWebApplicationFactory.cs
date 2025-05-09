using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Web.Api.Core.Configuration;
using Web.Api.Infrastructure.Data;
using Web.Api.Infrastructure.Data.Repositories;
using Web.Api.Infrastructure.Identity;
using Xunit;

namespace Web.Api.IntegrationTests;
public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>, IAsyncLifetime where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Route the application's logs to the xunit output
        builder.UseEnvironment("IntegrationTests");
        builder.ConfigureServices((context, services) =>
        {
            // Create a new service provider.
            services.AddLogging((loggingBuilder) =>
                loggingBuilder.SetMinimumLevel(LogLevel.Debug).AddXUnit());
            services.Configure<GrpcConfig>(context.Configuration.GetSection(nameof(GrpcConfig)));
            services.AddScoped<SignInManager<AppUser>>();
            services.AddScoped<ILogger<UserRepository>>(provider =>
            {
                ILoggerFactory loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                return loggerFactory.CreateLogger<UserRepository>();
            });
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

                // Ensure the database is created.
                await appDb.Database.EnsureCreatedAsync();
                await identityDb.Database.EnsureCreatedAsync();
                // Seed the database with test data.
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