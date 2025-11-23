using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Web.Api.Core.Configuration;
using Web.Api.Infrastructure.Data;
using Web.Api.Infrastructure.Identity;
using Web.Api.IntegrationTests;
using Xunit;
using ILogger = Microsoft.Extensions.Logging.ILogger;
// https://github.com/xunit/xunit/issues/3305
[assembly: AssemblyFixture(typeof(CustomWebApplicationFactory<Program>))]
namespace Web.Api.IntegrationTests;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>, IAsyncLifetime where TStartup : class
{
    public HttpClient Client { get; private set; }
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
            services.AddScoped<ILogger<CustomWebApplicationFactory<TStartup>>>(provider =>
                {
                    ILoggerFactory loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                    return loggerFactory.CreateLogger<CustomWebApplicationFactory<TStartup>>();
                });
        });
    }
    public async ValueTask InitializeAsync()
    {
        Client = CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost:4433"),
        });
        Client.Timeout = TimeSpan.FromSeconds(10);
        /* https://github.com/dotnet/aspnetcore/issues/61871
         * The HttpClient used with WebApplicationFactory uses an in-memory transport, so no actual network communication happens so I don't think it'll make any difference if you change it.
         */
        /*Client.DefaultRequestVersion = HttpVersion.Version30;
        Client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact;*/
        using (var scope = Services.CreateScope())
            try
            {
                var scopedServices = scope.ServiceProvider;
                var appDb = scopedServices.GetRequiredService<AppDbContext>();
                var identityDb = scopedServices.GetRequiredService<AppIdentityDbContext>();
                ILogger logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();
                //ILogger logger = loggerFactory.CreateLogger<CustomWebApplicationFactory<TStartup>>();
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