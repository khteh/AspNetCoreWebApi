using Microsoft.AspNetCore.TestHost;
using System.Threading.Tasks;
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