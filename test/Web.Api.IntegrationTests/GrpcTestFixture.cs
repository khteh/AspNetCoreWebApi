using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Web.Api.Core.Configuration;
namespace Web.Api.IntegrationTests;
public class GrpcTestFixture<TStartup> : IDisposable where TStartup : class
{
    private readonly WebApplicationFactory<TStartup> _factory;
    public LoggerFactory LoggerFactory { get; }
    public GrpcChannel GrpcChannel { get; }
    public GrpcTestFixture()
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
        _factory = new CustomGRPCWebApplicationFactory<TStartup>(config);
        var client = _factory.CreateDefaultClient(new ResponseVersionHandler());
        client.BaseAddress = new Uri(grpcConfig.Endpoint);
        GrpcChannel = GrpcChannel.ForAddress(client.BaseAddress, new GrpcChannelOptions
        {
            LoggerFactory = LoggerFactory,
            HttpClient = client
        });
    }
    public void Dispose()
    {
        _factory.Dispose();
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