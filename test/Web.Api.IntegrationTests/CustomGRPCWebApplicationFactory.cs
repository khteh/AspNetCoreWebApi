using Grpc.Net.Client;
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
using Web.Api.IntegrationTests.Services;

namespace Web.Api.IntegrationTests;

public class CustomGRPCWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>, IDisposable where TStartup : class
{
    private GrpcChannel _grpcChannel;
    private IServiceCollection _services;
    public GrpcChannel GrpcChannel { get => _grpcChannel; }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            GrpcConfig grpcConfig = context.Configuration.GetSection(nameof(GrpcConfig)).Get<GrpcConfig>();
            var client = CreateDefaultClient(new Http3Handler());
            client.BaseAddress = new Uri(grpcConfig.Endpoint);
            _grpcChannel = GrpcChannel.ForAddress(client.BaseAddress, new GrpcChannelOptions
            {
                LoggerFactory = new LoggerFactory(),
                HttpClient = client
            });
            services.AddDbContextPool<AppIdentityDbContext>(options =>
            {
                options.UseNpgsql(context.Configuration.GetConnectionString("IntegrationTests"), o => o.MigrationsAssembly("Web.Api.Infrastructure"));
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                options.LogTo(Console.WriteLine);
            })
                .AddDbContextPool<AppDbContext>(options =>
                {
                    options.UseNpgsql(context.Configuration.GetConnectionString("IntegrationTests"), o => o.MigrationsAssembly("Web.Api.Infrastructure"));
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
            services.Configure<GrpcConfig>(context.Configuration.GetSection(nameof(GrpcConfig)));
            // Build the service provider.
            _services = services;
            IServiceProvider ServiceProvider = services.BuildServiceProvider();
            // Create a scope to obtain a reference to the database contexts
            using (var scope = ServiceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var appDb = scopedServices.GetRequiredService<AppDbContext>();
                var identityDb = scopedServices.GetRequiredService<AppIdentityDbContext>();
                var logger = scopedServices.GetRequiredService<ILogger<CustomGRPCWebApplicationFactory<TStartup>>>();
                // Ensure the database is created.
                appDb.Database.EnsureCreated();
                identityDb.Database.EnsureCreated();
                try
                {
                    // Seed the database with test data.
                    SeedData.PopulateGrpcTestData(identityDb, appDb);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"An error occurred seeding the database with test messages. Error: {ex.Message}");
                }
            }
        });
    }
    public new void Dispose()
    {
        var sp = _services.BuildServiceProvider();
        // Create a scope to obtain a reference to the database contexts
        using (var scope = sp.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var appDb = scopedServices.GetRequiredService<AppDbContext>();
            var identityDb = scopedServices.GetRequiredService<AppIdentityDbContext>();
            SeedData.CleanUpGrpcTestData(identityDb, appDb);
        }
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}