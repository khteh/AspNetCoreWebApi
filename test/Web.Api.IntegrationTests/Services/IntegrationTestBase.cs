namespace Web.Api.IntegrationTests.Services;

using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using System;
using Xunit;

public class IntegrationTestBase : IClassFixture<GrpcTestFixture<Program>>, IDisposable
{
    protected ITestOutputHelper _output;
    private GrpcChannel? _channel;
    private IDisposable? _testContext;
    protected GrpcTestFixture<Program> Fixture { get; set; }
    protected ILoggerFactory LoggerFactory => Fixture.LoggerFactory;
    protected GrpcChannel Channel => _channel ??= CreateChannel();

    protected GrpcChannel CreateChannel()
    {
        //return GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
        return GrpcChannel.ForAddress(new Uri(Fixture.GrpcConfig.Endpoint), new GrpcChannelOptions
        {
            LoggerFactory = LoggerFactory,
            HttpHandler = new Http3Handler(Fixture.Handler)
            //HttpHandler = Fixture.Handler
        });
    }

    public IntegrationTestBase(GrpcTestFixture<Program> fixture, ITestOutputHelper output)
    {
        _output = output;
        Fixture = fixture;
        _testContext = Fixture.GetTestContext(_output);
    }

    public void Dispose()
    {
        _testContext?.Dispose();
        _channel = null;
    }
}
