using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;
namespace Web.Api.IntegrationTests.Controllers;
public class ProtectedControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    public ProtectedControllerIntegrationTests(CustomWebApplicationFactory<Program> factory) => _client = factory.CreateClient();
    [Fact]
    public async Task CantAccessProtectedResourceWithoutLogin()
    {
        var httpResponse = await _client.GetAsync("api/protected/home");
        Assert.Equal(HttpStatusCode.Unauthorized, httpResponse.StatusCode);
    }
    [Fact]
    public async Task CanAccessProtectedResourceAfterLogin()
    {
        var httpResponse = await _client.PostAsync("/api/auth/login", new StringContent(System.Text.Json.JsonSerializer.Serialize(new Models.Request.LogInRequest("mickeymouse", "P@$$w0rd")), Encoding.UTF8, "application/json"));
        httpResponse.EnsureSuccessStatusCode();
        var stringResponse = await httpResponse.Content.ReadAsStringAsync();
        JsonNode result = JsonNode.Parse(stringResponse);
        Assert.NotNull(result["accessToken"]);
        Assert.False(string.IsNullOrEmpty((string)result["accessToken"]["token"]));
        Assert.Equal(7200, (int)result["accessToken"]["expiresIn"]);
        Assert.False(string.IsNullOrEmpty((string)result["refreshToken"]));

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", (string)result["accessToken"]["token"]);
        var httpResponse1 = await _client.GetAsync("api/protected/home");
        Assert.Equal(HttpStatusCode.OK, httpResponse1.StatusCode);
    }
}