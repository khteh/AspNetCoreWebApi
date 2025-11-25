using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Web.Api.Core.Configuration;
using Web.Api.Infrastructure.Data;
using Web.Api.Infrastructure.Data.Repositories;
using Web.Api.Infrastructure.Identity;
using Web.Api.IntegrationTests;
using Web.Api.IntegrationTests.Services;
using Xunit;
using static System.Console;

// https://github.com/xunit/xunit/issues/3305
//[assembly: AssemblyFixture(typeof(CustomGRPCWebApplicationFactory<Program>))]
namespace Web.Api.IntegrationTests;

public class CustomGRPCWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>, IAsyncLifetime where TStartup : class
{
    private GrpcChannel _grpcChannel;
    public GrpcChannel GrpcChannel { get => _grpcChannel; }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Route the application's logs to the xunit output
        builder.UseEnvironment("IntegrationTests");
        builder.ConfigureLogging((p) => p.SetMinimumLevel(LogLevel.Debug));
        builder.UseTestServer();
        builder.ConfigureServices((context, services) =>
        {
            GrpcConfig grpcConfig = context.Configuration.GetSection(nameof(GrpcConfig)).Get<GrpcConfig>();
            var client = CreateDefaultClient(new Http3Handler());
            client.BaseAddress = new Uri(grpcConfig.Endpoint);
            Server.BaseAddress = new Uri(grpcConfig.Endpoint);
            _grpcChannel = GrpcChannel.ForAddress(client.BaseAddress, new GrpcChannelOptions
            {
                LoggerFactory = new LoggerFactory(),
                HttpHandler = new Http3Handler(Server.CreateHandler())
            });
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
        // Create a scope to obtain a reference to the database contexts
        using (var scope = Services.CreateScope())
        try
        {
            var scopedServices = scope.ServiceProvider;
            var appDb = scopedServices.GetRequiredService<AppDbContext>();
            var identityDb = scopedServices.GetRequiredService<AppIdentityDbContext>();
            var logger = scopedServices.GetRequiredService<ILogger<CustomGRPCWebApplicationFactory<TStartup>>>();
            // Ensure the database is created.
            appDb.Database.EnsureCreated();
            identityDb.Database.EnsureCreated();
            // Seed the database with test data.
            logger.LogDebug($"{nameof(InitializeAsync)} populate test data...");
            await SeedData.PopulateGrpcTestData(identityDb, appDb);
        }
        catch (Exception ex)
        {
            WriteLine($"{nameof(InitializeAsync)} exception! {ex}");
        }
    }
    public async override ValueTask DisposeAsync()
    {
        // Create a scope to obtain a reference to the database contexts
        using (var scope = Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var appDb = scopedServices.GetRequiredService<AppDbContext>();
            var identityDb = scopedServices.GetRequiredService<AppIdentityDbContext>();
            await SeedData.CleanUpGrpcTestData(identityDb, appDb);
        }
        await base.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}