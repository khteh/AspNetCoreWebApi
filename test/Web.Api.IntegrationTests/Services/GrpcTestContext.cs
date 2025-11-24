namespace Web.Api.IntegrationTests.Services;

using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using Xunit;

internal class GrpcTestContext<TStartup> : IDisposable where TStartup : class
{
    private readonly Stopwatch _stopwatch;
    private readonly GrpcTestFixture<TStartup> _fixture;
    private readonly ITestOutputHelper _outputHelper;
    public GrpcTestContext(GrpcTestFixture<TStartup> fixture, ITestOutputHelper outputHelper)
    {
        _stopwatch = Stopwatch.StartNew();
        _fixture = fixture;
        _outputHelper = outputHelper;
        _fixture.LoggedMessage += WriteMessage;
    }
    private void WriteMessage(LogLevel logLevel, string category, EventId eventId, string message, Exception? exception)
    {
        var log = $"{_stopwatch.Elapsed.TotalSeconds:N3}s {category} - {logLevel}: {message}";
        if (exception != null)
            log += Environment.NewLine + exception.ToString();
        _outputHelper.WriteLine(log);
    }
    public void Dispose() => _fixture.LoggedMessage -= WriteMessage;
}
