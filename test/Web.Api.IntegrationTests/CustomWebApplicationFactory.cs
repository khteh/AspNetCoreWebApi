using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.Testing.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Web.Api.Core.Configuration;
using Web.Api.Infrastructure.Data;
using Web.Api.Infrastructure.Identity;
using Web.Api.IntegrationTests;
using Web.Api.IntegrationTests.Controllers;
using Xunit;
using ILogger = Microsoft.Extensions.Logging.ILogger;
// https://github.com/xunit/xunit/issues/3305
[assembly: AssemblyFixture(typeof(CustomWebApplicationFactory<Program>))]
namespace Web.Api.IntegrationTests;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>, IAsyncLifetime where TStartup : class
{
    private CookieContainerHandler _cookieContainerHandler;
    public HttpClient Client { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="TestServerFixture"/> class.
    /// </summary>
    public CustomWebApplicationFactory() : base()
    {
        ClientOptions.BaseAddress = new Uri("https://localhost:4433");
        ClientOptions.AllowAutoRedirect = false;
        ClientOptions.HandleCookies = true;
    }
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
        builder.ConfigureAntiforgeryTokenResource();
    }
    /// <summary>
    /// Gets a set of valid antiforgery tokens for the application as an asynchronous operation.
    /// </summary>
    /// <param name="httpClientFactory">
    /// An optional delegate to a method to provide an <see cref="HttpClient"/> to use to obtain the response.
    /// </param>
    /// <param name="cancellationToken">
    /// The optional <see cref="CancellationToken"/> to use for the HTTP request to obtain the response.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation to get a set of valid
    /// antiforgery (CSRF/XSRF) tokens to use for HTTP POST requests to the test server.
    /// </returns>
    public async Task<AntiforgeryTokens> GetAntiforgeryTokensAsync(
        Func<HttpClient>? httpClientFactory = null,
        CancellationToken cancellationToken = default)
    {
        using var httpClient = httpClientFactory?.Invoke() ?? CreateClient();

        var tokens = await httpClient.GetFromJsonAsync<AntiforgeryTokens>(
            AntiforgeryTokenController.GetTokensUri,
            cancellationToken);

        return tokens!;
    }
    public async ValueTask InitializeAsync()
    {
        /*
        Client = CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost:4433"),
            AllowAutoRedirect = false,
            HandleCookies = true
        });
        */
        // https://blog.martincostello.com/integration-testing-antiforgery-with-application-parts/
        // Arrange - Get valid CSRF tokens and parameter names from the server
        AntiforgeryTokens tokens = await GetAntiforgeryTokensAsync(cancellationToken: TestContext.Current.CancellationToken);
        // Configure a handler with the CSRF cookie
        _cookieContainerHandler = new CookieContainerHandler();
        _cookieContainerHandler.Container.Add(
            Server.BaseAddress,
            new Cookie(tokens.CookieName, tokens.CookieValue));
        // Create an HTTP client and add the CSRF cookie
        Client = CreateDefaultClient(_cookieContainerHandler);

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