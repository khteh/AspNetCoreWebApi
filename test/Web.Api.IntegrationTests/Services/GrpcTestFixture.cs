namespace Web.Api.IntegrationTests.Services;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using Web.Api.Core.Configuration;
using Web.Api.Infrastructure.Data;
using Web.Api.Infrastructure.Data.Repositories;
using Web.Api.Infrastructure.Identity;
using Xunit;

public delegate void LogMessage(LogLevel logLevel, string categoryName, EventId eventId, string message, Exception? exception);

public class GrpcTestFixture<TStartup> : IDisposable where TStartup : class
{
    private IServiceCollection _services;
    private TestServer? _server;
    private IHost? _host;
    private HttpMessageHandler? _handler;
    private Action<IWebHostBuilder>? _configureWebHost;
    public event LogMessage? LoggedMessage;
    private string _connectionString;
    private readonly GrpcConfig _grpcConfig;
    public GrpcConfig GrpcConfig { get => _grpcConfig; }
    private readonly ILogger<GrpcTestFixture<TStartup>> _logger;
    public GrpcTestFixture()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTests");
        string contentRootFull = Path.GetFullPath(Directory.GetCurrentDirectory());
        IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(contentRootFull)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.Development.json", true, true)
                .AddEnvironmentVariables().Build();
        _grpcConfig = config.GetSection(nameof(GrpcConfig)).Get<GrpcConfig>();
        _connectionString = config.GetConnectionString("IntegrationTests");
        LoggerFactory = new LoggerFactory();
        LoggerFactory.AddProvider(new ForwardingLoggerProvider((logLevel, category, eventId, message, exception) =>
        {
            LoggedMessage?.Invoke(logLevel, category, eventId, message, exception);
        }));
        _logger = LoggerFactory.CreateLogger<GrpcTestFixture<TStartup>>();
    }

    public void ConfigureWebHost(Action<IWebHostBuilder> configure) => _configureWebHost = configure;

    private void EnsureServer()
    {
        if (_host == null)
            try
            {
                var builder = new HostBuilder()
                    .ConfigureServices((context, services) =>
                    {
                        services.AddSingleton<ILoggerFactory>(LoggerFactory);
                        services.AddDbContextPool<AppIdentityDbContext>(options =>
                        {
                            options.UseNpgsql(_connectionString, o => o.SetPostgresVersion(18, 0));
                            options.EnableSensitiveDataLogging();
                            options.EnableDetailedErrors();
                            options.LogTo(Console.WriteLine);
                        })
                            .AddDbContextPool<AppDbContext>(options =>
                            {
                                options.UseNpgsql(_connectionString, o => o.SetPostgresVersion(18, 0));
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
                        _services = services;
                        // Build the service provider.
                        IServiceProvider ServiceProvider = services.BuildServiceProvider();
                        // Create a scope to obtain a reference to the database contexts
                        using (var scope = ServiceProvider.CreateScope())
                        {
                            var scopedServices = scope.ServiceProvider;
                            var appDb = scopedServices.GetRequiredService<AppDbContext>();
                            var identityDb = scopedServices.GetRequiredService<AppIdentityDbContext>();
                            var logger = scopedServices.GetRequiredService<ILogger<GrpcTestFixture<TStartup>>>();
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
                    })
                    .ConfigureWebHostDefaults(webHost =>
                    {
                        webHost.UseTestServer().UseStartup<TStartup>();
                        _configureWebHost?.Invoke(webHost);
                    });
                _host = builder.Start();
                _server = _host.GetTestServer();
                _server.BaseAddress = new Uri(GrpcConfig.Endpoint);
                _handler = _server.CreateHandler();
            }
            catch (Exception e)
            {
                _logger.LogCritical($"Exception! {e.Message}");
            }
    }

    public LoggerFactory LoggerFactory { get; }

    public HttpMessageHandler Handler
    {
        get
        {
            EnsureServer();
            return _handler!;
        }
    }

    public void Dispose()
    {
        _handler?.Dispose();
        _host?.Dispose();
        _server?.Dispose();
#if false        
        var sp = _services.BuildServiceProvider();
        // Create a scope to obtain a reference to the database contexts
        using (var scope = sp.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var appDb = scopedServices.GetRequiredService<AppDbContext>();
            var identityDb = scopedServices.GetRequiredService<AppIdentityDbContext>();
            SeedData.CleanUpGrpcTestData(identityDb, appDb);
        }
#endif
        GC.SuppressFinalize(this);
    }

    public IDisposable GetTestContext(ITestOutputHelper outputHelper)
    {
        return new GrpcTestContext<TStartup>(this, outputHelper);
    }
}
