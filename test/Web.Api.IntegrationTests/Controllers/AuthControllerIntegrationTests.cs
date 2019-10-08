using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            var httpResponse = await _client.PostAsync("/api/auth/login", new StringContent(JsonConvert.SerializeObject(new Models.Request.LoginRequest("mickeymouse", "P@$$w0rd")), Encoding.UTF8, "application/json"));
            httpResponse.EnsureSuccessStatusCode();
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            dynamic result = JObject.Parse(stringResponse);
            Assert.NotNull(result.accessToken.token);
            Assert.NotNull(result.refreshToken);
            Assert.Equal(7200,(int)result.accessToken.expiresIn);
        }

        [Fact]
        public async Task CantLoginWithInvalidCredentials()
        {
            var httpResponse = await _client.PostAsync("/api/auth/login", new StringContent(JsonConvert.SerializeObject(new Models.Request.LoginRequest("unknown", "Rhcp1234")), Encoding.UTF8, "application/json"));
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            Assert.Contains("Invalid username or password!", stringResponse);
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponse.StatusCode);
        }

        [Fact]
        public async Task CanExchangeValidRefreshToken()
        {
            var httpResponse = await _client.PostAsync("/api/auth/login", new StringContent(JsonConvert.SerializeObject(new Models.Request.LoginRequest("mickeymouse", "P@$$w0rd")), Encoding.UTF8, "application/json"));
            httpResponse.EnsureSuccessStatusCode();
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            dynamic result = JObject.Parse(stringResponse);
            Assert.NotNull(result.accessToken.token);
            Assert.NotNull(result.refreshToken);
            Assert.Equal(7200,(int)result.accessToken.expiresIn);

            var refreshTokenResponse = await _client.PostAsync("/api/auth/refreshtoken", new StringContent(JsonConvert.SerializeObject(new Models.Request.ExchangeRefreshTokenRequest {
                AccessToken = result.accessToken.token,
                RefreshToken = result.refreshToken
            }), Encoding.UTF8, "application/json"));
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
            var httpResponse = await _client.PostAsync("/api/auth/refreshtoken", new StringContent(JsonConvert.SerializeObject(new Models.Request.ExchangeRefreshTokenRequest { AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJtbWFjbmVpbCIsImp0aSI6IjA0YjA0N2E4LTViMjMtNDgwNi04M2IyLTg3ODVhYmViM2ZjNyIsImlhdCI6MTUzOTUzNzA4Mywicm9sIjoiYXBpX2FjY2VzcyIsImlkIjoiNDE1MzI5NDUtNTk5ZS00OTEwLTk1OTktMGU3NDAyMDE3ZmJlIiwibmJmIjoxNTM5NTM3MDgyLCJleHAiOjE1Mzk1NDQyODIsImlzcyI6IndlYkFwaSIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMC8ifQ.xzDQOKzPZarve68Np8Iu8sh2oqoCpHSmp8fMdYRHC_k", RefreshToken = "unknown" }), Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
            var strResponse = await httpResponse.Content.ReadAsStringAsync();
            Models.Response.ExchangeRefreshTokenResponse response = Web.Api.Serialization.JsonSerializer.DeSerializeObject<Models.Response.ExchangeRefreshTokenResponse>(strResponse);
            Assert.NotNull(response);
            Assert.Null(response.AccessToken);
            Assert.True(string.IsNullOrEmpty(response.RefreshToken));
            Assert.NotNull(response.Errors);
            Assert.NotEmpty(response.Errors);
            Assert.Equal("InvalidToken", response.Errors.First().Code);
            Assert.Equal("Invalid token!", response.Errors.First().Description);
        }
    }
}