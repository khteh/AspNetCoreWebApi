using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Web.Api.Models.Response;
using Xunit;
namespace Web.Api.IntegrationTests.SignalR;
[Collection("Controller Test Collection")]
public class Tests
{
    private readonly TestServer _testServer;
    public Tests(CustomWebApplicationFactory<Program> factory) => _testServer = factory.Server;
    [Fact]
    public async Task Test()
    {
        Assert.True(true);
    }
}