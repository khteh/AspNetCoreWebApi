using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Web.Api.IntegrationTests.Controllers
{
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
            var httpResponse = await _client.PostAsync("/api/auth/login", new StringContent(System.Text.Json.JsonSerializer.Serialize(new Models.Request.LoginRequest("mickeymouse", "P@$$w0rd")), Encoding.UTF8, "application/json"));
            httpResponse.EnsureSuccessStatusCode();
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            JsonDocument result = JsonDocument.Parse(stringResponse);
            Assert.True(result.RootElement.TryGetProperty("accessToken", out JsonElement accessToken));
            Assert.True(accessToken.TryGetProperty("token", out JsonElement token));
            Assert.True(accessToken.TryGetProperty("expiresIn", out JsonElement expiry));
            Assert.True(result.RootElement.TryGetProperty("refreshToken", out JsonElement refreshToken));
            Assert.False(string.IsNullOrEmpty(token.GetString()));
            Assert.Equal(7200, expiry.GetInt32());
            Assert.False(string.IsNullOrEmpty(refreshToken.GetString()));

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.GetString());
            var httpResponse1 = await _client.GetAsync("api/protected/home");
            Assert.Equal(HttpStatusCode.OK, httpResponse1.StatusCode);
        }
    }
}