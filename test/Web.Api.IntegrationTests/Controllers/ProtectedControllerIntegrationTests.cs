using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Web.Api.IntegrationTests.Controllers
{
    public class ProtectedControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        public ProtectedControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory) => _client = factory.CreateClient();
        [Fact]
        public async Task CantAccessProtectedResourceWithoutLogin()
        {
            var httpResponse = await _client.GetAsync("api/protected/home");
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponse.StatusCode);
        }
        [Fact]
        public async Task CanAccessProtectedResourceAfterLogin()
        {
            var httpResponse = await _client.PostAsync("/api/auth/login", new StringContent(JsonConvert.SerializeObject(new Models.Request.LoginRequest("mickeymouse", "P@$$w0rd")), Encoding.UTF8, "application/json"));
            httpResponse.EnsureSuccessStatusCode();
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            dynamic result = JObject.Parse(stringResponse);
            Assert.NotNull(result.accessToken.token);
            Assert.NotNull(result.refreshToken);
            Assert.Equal(7200,(int)result.accessToken.expiresIn);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", (string)result.accessToken.token);
            var httpResponse1 = await _client.GetAsync("api/protected/home");
            Assert.Equal(HttpStatusCode.OK, httpResponse1.StatusCode);
        }
    }
}