using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;
using Web.Api.Core.Configuration;
using Web.Api.Infrastructure.Data;
using Web.Api.Infrastructure.Identity;
using Web.Api.IntegrationTests;
using Web.Api.IntegrationTests.Services;
using Xunit;
using static System.Console;
using ILogger = Microsoft.Extensions.Logging.ILogger;
// https://github.com/xunit/xunit/issues/3305
[assembly: AssemblyFixture(typeof(CustomWebApplicationFactory<Program>))]
namespace Web.Api.IntegrationTests;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>, IAsyncLifetime where TStartup : class
{
    public HttpClient Client { get; private set; }
    private GrpcConfig _grpcConfig;
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
            _grpcConfig = context.Configuration.GetSection(nameof(GrpcConfig)).Get<GrpcConfig>();
            // Create a new service provider.
            services.Configure<GrpcConfig>(context.Configuration.GetSection(nameof(GrpcConfig)));
            services.AddScoped<SignInManager<AppUser>>();
            services.AddScoped<ILogger<CustomWebApplicationFactory<TStartup>>>(provider =>
                {
                    ILoggerFactory loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                    return loggerFactory.CreateLogger<CustomWebApplicationFactory<TStartup>>();
                });
            services.AddHttpClient("AspNetCoreWebApi",  httpClient => {
                httpClient.BaseAddress = new Uri("https://localhost:4433");
                httpClient.DefaultRequestVersion = HttpVersion.Version30;
                httpClient.Timeout = TimeSpan.FromSeconds(10);
            })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    // Create and configure your custom HttpClientHandler
                    return new HttpClientHandler
                    {
                        AllowAutoRedirect = true, // Example customization
                        UseDefaultCredentials = true,
                        UseCookies = false, // Another example customization
                        CheckCertificateRevocationList = false,
                        //ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        //ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                        ServerCertificateCustomValidationCallback = (sender, cert, chain, errors) =>
                        {
                            WriteLine($"SSL Errors: {errors}");
                            foreach (var status in chain.ChainStatus)
                                WriteLine($"Chain Status: {status.Status} - {status.StatusInformation}");
                            return true; // Temporary for testing
                        }
                    };
                });
        });
    }
    public async ValueTask InitializeAsync()
    {
        // If you need explicit certificate validation handling in tests:
        var handler = new HttpClientHandler() {
            AllowAutoRedirect = true, // Example customization
            UseDefaultCredentials = true,
            UseCookies = false, // Another example customization
            CheckCertificateRevocationList = false,
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        var handler1 = new HttpClientHandler()
        {
            AllowAutoRedirect = true, // Example customization
            UseDefaultCredentials = true,
            UseCookies = false, // Another example customization
            CheckCertificateRevocationList = false,
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                // Logic to accept your self-signed certificate
                return errors == SslPolicyErrors.None || cert.Subject.Contains("AspNetCoreWebApi");
            }
        };
        var handler2 = Server.CreateHandler();
        // Client = new HttpClient(...) No connection could be made because the target machine actively refused it. (localhost:4433)
        Client = CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost:4433")
        });
        Client.DefaultRequestVersion = HttpVersion.Version30;
        /*
         * https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/http3
         * Client.DefaultRequestVersion = HttpVersion.Version30; // Configure for HTTP/3
         * Client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact; // Ensure HTTP/3 is used
        */
        Client.Timeout = TimeSpan.FromSeconds(10);

        Server.BaseAddress = new Uri(_grpcConfig.Endpoint);
        _grpcChannel = GrpcChannel.ForAddress(Server.BaseAddress, new GrpcChannelOptions
        {
            HttpVersion = HttpVersion.Version30,
            LoggerFactory = new LoggerFactory(),
            //HttpHandler = new Http3Handler(Server.CreateHandler())
            HttpHandler = new Http3Handler(handler2)
            //HttpHandler = handler
            //HttpHandler = Server.CreateHandler()
        });
        /* https://github.com/dotnet/aspnetcore/issues/61871
         * The HttpClient used with WebApplicationFactory uses an in-memory transport, so no actual network communication happens so I don't think it'll make any difference if you change it.
         */
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
                WriteLine($"{nameof(InitializeAsync)} exception! {ex}");
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