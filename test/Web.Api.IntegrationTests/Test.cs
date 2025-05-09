using Microsoft.AspNetCore.TestHost;
using System.Threading.Tasks;
using Web.Api.IntegrationTests.Controllers;
using Xunit;
namespace Web.Api.IntegrationTests.SignalR;
[Collection(SignalRTestsCollection.Name)]
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