using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Web.Api.Core.Configuration;
namespace Web.Api.IntegrationTests
{
    public class GrpcTestFixture<TStartup> : IDisposable where TStartup : class
    {
        private readonly TestServer _server;
        private readonly IHost _host;
        public LoggerFactory LoggerFactory { get; }
        public HttpClient Client { get; }
        public GrpcTestFixture() : this(null) { }
        public GrpcTestFixture(Action<IServiceCollection>? initialConfigureServices)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTests");
            string contentRootFull = Path.GetFullPath(Directory.GetCurrentDirectory());
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(contentRootFull)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.Development.json", true, true)
                .AddEnvironmentVariables().Build();
            GrpcConfig grpcConfig = config.GetSection(nameof(GrpcConfig)).Get<GrpcConfig>();
            LoggerFactory = new LoggerFactory();
            var builder = new HostBuilder()
                .ConfigureServices(services =>
                {
                    initialConfigureServices?.Invoke(services);
                    services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
                })
                .ConfigureWebHostDefaults(webHost =>
                {
                    webHost.UseTestServer().UseStartup<TStartup>();
                });
            _host = builder.Start();
            _server = _host.GetTestServer();
            // Need to set the response version to 2.0.
            // Required because of this TestServer issue - https://github.com/aspnet/AspNetCore/issues/16940
            var responseVersionHandler = new ResponseVersionHandler();
            responseVersionHandler.InnerHandler = _server.CreateHandler();
            var client = new HttpClient(responseVersionHandler);
            client.BaseAddress = new Uri(grpcConfig.Endpoint);
            Client = client;
        }
        public void Dispose()
        {
            Client.Dispose();
            _host.Dispose();
            _server.Dispose();
            GC.SuppressFinalize(this);
        }
        private class ResponseVersionHandler : DelegatingHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = await base.SendAsync(request, cancellationToken);
                response.Version = request.Version;
                return response;
            }
        }
    }
}