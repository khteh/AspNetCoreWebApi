using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Web.Api.IntegrationTests.Controllers
{
    public class AuthControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        public AuthControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory) => _client = factory.CreateClient();

        [Fact]
        public async Task CanLoginWithValidCredentials()
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
        }

        [Fact]
        public async Task CantLoginWithInvalidCredentials()
        {
            var httpResponse = await _client.PostAsync("/api/auth/login", new StringContent(System.Text.Json.JsonSerializer.Serialize(new Models.Request.LoginRequest("unknown", "Rhcp1234")), Encoding.UTF8, "application/json"));
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            Assert.Contains("Invalid username or password!", stringResponse);
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponse.StatusCode);
        }

        [Fact]
        public async Task CanExchangeValidRefreshToken()
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

            var refreshTokenResponse = await _client.PostAsync("/api/auth/refreshtoken", new StringContent(JsonSerializer.Serialize(new Models.Request.ExchangeRefreshTokenRequest(token.GetString(), refreshToken.GetString())), Encoding.UTF8, "application/json"));
            refreshTokenResponse.EnsureSuccessStatusCode();
            var strRefreshTokenResponse = await refreshTokenResponse.Content.ReadAsStringAsync();
            Models.Response.ExchangeRefreshTokenResponse response = Web.Api.Serialization.JsonSerializer.DeSerializeObject<Models.Response.ExchangeRefreshTokenResponse>(strRefreshTokenResponse);
            Assert.NotNull(response);
            Assert.NotNull(response.AccessToken);
            Assert.NotNull(response.AccessToken.Token);
            Assert.False(string.IsNullOrEmpty(response.RefreshToken));
            Assert.Equal(7200, (int)response.AccessToken.ExpiresIn);
            Assert.Null(response.Errors);
        }

        [Fact]
        public async Task CantExchangeInvalidRefreshToken()
        {
            var httpResponse = await _client.PostAsync("/api/auth/refreshtoken", new StringContent(JsonSerializer.Serialize(new Models.Request.ExchangeRefreshTokenRequest("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJtbWFjbmVpbCIsImp0aSI6IjA0YjA0N2E4LTViMjMtNDgwNi04M2IyLTg3ODVhYmViM2ZjNyIsImlhdCI6MTUzOTUzNzA4Mywicm9sIjoiYXBpX2FjY2VzcyIsImlkIjoiNDE1MzI5NDUtNTk5ZS00OTEwLTk1OTktMGU3NDAyMDE3ZmJlIiwibmJmIjoxNTM5NTM3MDgyLCJleHAiOjE1Mzk1NDQyODIsImlzcyI6IndlYkFwaSIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMC8ifQ.xzDQOKzPZarve68Np8Iu8sh2oqoCpHSmp8fMdYRHC_k", "unknown")), Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
            var strResponse = await httpResponse.Content.ReadAsStringAsync();
            Models.Response.ExchangeRefreshTokenResponse response = Web.Api.Serialization.JsonSerializer.DeSerializeObject<Models.Response.ExchangeRefreshTokenResponse>(strResponse);
            Assert.NotNull(response);
            Assert.Null(response.AccessToken);
            Assert.True(string.IsNullOrEmpty(response.RefreshToken));
            Assert.NotNull(response.Errors);
            Assert.NotEmpty(response.Errors);
            Assert.Equal(HttpStatusCode.BadRequest.ToString(), response.Errors.First().Code);
            Assert.Equal("Invalid token!", response.Errors.First().Description);
        }
    }
}