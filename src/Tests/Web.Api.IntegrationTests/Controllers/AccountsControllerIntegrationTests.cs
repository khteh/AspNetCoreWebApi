using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Web.Api.Core.DTO.UseCaseRequests;
using Xunit;

namespace Web.Api.IntegrationTests.Controllers
{
    public class AccountsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public AccountsControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CanRegisterUserWithValidAccountDetails()
        {
            var httpResponse = await _client.PostAsync("/api/accounts/register", new StringContent(JsonConvert.SerializeObject(new Models.Request.RegisterUserRequest("John", "Doe", "jdoe@gmail.com", "johndoe", "Pa$$word1")), Encoding.UTF8, "application/json"));
            httpResponse.EnsureSuccessStatusCode();
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            dynamic result = JObject.Parse(stringResponse);
            Assert.True((bool) result.success);
            Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);
            Assert.Null(result.Errors);
            Assert.False(string.IsNullOrEmpty((string)result.id));
        }
        [Fact]
        public async Task CanDeleteUserWithValidAccountDetails()
        {
            var httpResponse = await _client.DeleteAsync("/api/accounts/deleteme");
            httpResponse.EnsureSuccessStatusCode();
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            dynamic result = JObject.Parse(stringResponse);
            Assert.True((bool)result.success);
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        }

        [Fact]
        public async Task CantRegisterUserWithInvalidAccountDetails()
        {
            var httpResponse = await _client.PostAsync("/api/accounts/register", new StringContent(JsonConvert.SerializeObject(new Models.Request.RegisterUserRequest("John", "Doe", "", "johndoe", "Pa$$word1")), Encoding.UTF8, "application/json"));
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            Assert.Contains("'Email' is not a valid email address.", stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        }
        [Fact]
        public async Task CantDeleteUserWithInvalidAccountDetails()
        {
            var httpResponse = await _client.DeleteAsync("/api/accounts/DeleteMeNot"); // UserManager is NOT case sensitive!
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            Assert.Contains("Invalid user!", stringResponse);
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        }
        [Fact]
        public async Task CanFindById()
        {
            var httpResponse = await _client.GetAsync("/api/accounts/id/41532945-599e-4910-9599-0e7402017fbe"); // UserManager is NOT case sensitive!
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            dynamic result = JObject.Parse(stringResponse);
            Assert.True((bool)result.success);
            Assert.False(string.IsNullOrEmpty((string)result.id));
            Assert.Equal("41532945-599e-4910-9599-0e7402017fbe", (string)result.id);
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        }
        [Fact]
        public async Task CanFindByUsername()
        {
            var httpResponse = await _client.GetAsync("/api/accounts/username/mickeymouse"); // UserManager is NOT case sensitive!
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            dynamic result = JObject.Parse(stringResponse);
            Assert.True((bool)result.success);
            Assert.False(string.IsNullOrEmpty((string)result.id));
            Assert.Equal("41532945-599e-4910-9599-0e7402017fbe", (string)result.id);
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        }
        [Fact]
        public async Task CanFindByEmail()
        {
            //var httpResponse = await _client.GetAsync(WebUtility.UrlEncode("/api/accounts/email/mickey@mouse.com")); // UserManager is NOT case sensitive!
            var httpResponse = await _client.GetAsync("/api/accounts/email/mickey@mouse.com"); // UserManager is NOT case sensitive!
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            dynamic result = JObject.Parse(stringResponse);
            Assert.True((bool)result.success);
            Assert.False(string.IsNullOrEmpty((string)result.id));
            Assert.Equal("41532945-599e-4910-9599-0e7402017fbe", (string)result.id);
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        }
        [Fact]
        public async Task CanChangePasswordWithValidAccountDetails()
        {
            // Create User
            var httpResponse = await _client.PostAsync("/api/accounts/register", new StringContent(JsonConvert.SerializeObject(new Models.Request.RegisterUserRequest("FirstName", "LastName", "user@gmail.com", "user1", "Pa$$word1")), Encoding.UTF8, "application/json"));
            httpResponse.EnsureSuccessStatusCode();
            dynamic result = JObject.Parse(await httpResponse.Content.ReadAsStringAsync());
            Assert.True((bool) result.success);
            Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);
            Assert.False(string.IsNullOrEmpty((string)result.id));

            // Login
            var loginResponse = await _client.PostAsync("/api/auth/login", new StringContent(JsonConvert.SerializeObject(new Models.Request.LoginRequest("user1", "Pa$$word1")), Encoding.UTF8, "application/json"));
            loginResponse.EnsureSuccessStatusCode();
            var strLoginSuccessResponse = await loginResponse.Content.ReadAsStringAsync();
            dynamic loginResult = JObject.Parse(strLoginSuccessResponse);
            Assert.NotNull(loginResult.accessToken.token);
            Assert.Equal(7200,(int)loginResult.accessToken.expiresIn);
            Assert.NotNull(loginResult.refreshToken);

            // Change Password
            var pwdResponse = await _client.PostAsync("/api/accounts/changepassword", new StringContent(JsonConvert.SerializeObject(new Models.Request.ChangePasswordRequest((string)result.id, "Pa$$word1", "Pa$$word2")), Encoding.UTF8, "application/json"));
            pwdResponse.EnsureSuccessStatusCode();
            dynamic pwdResult = JObject.Parse(await pwdResponse.Content.ReadAsStringAsync());
            Assert.True((bool) pwdResult.success);
            Assert.Equal(HttpStatusCode.OK, pwdResponse.StatusCode);
            Assert.False(string.IsNullOrEmpty((string)pwdResult.id));
            Assert.Equal(result.Id, pwdResult.Id);

            // Should fail login with previous password
            var loginFailResponse = await _client.PostAsync("/api/auth/login", new StringContent(JsonConvert.SerializeObject(new Models.Request.LoginRequest("user1", "Pa$$W0rd1")), Encoding.UTF8, "application/json"));
            var strLoginFailResponse = await loginFailResponse.Content.ReadAsStringAsync();
            Assert.Contains("Invalid username or password!", strLoginFailResponse);
            Assert.Equal(HttpStatusCode.Unauthorized, loginFailResponse.StatusCode);

            // Login
            var loginSuccessResponse = await _client.PostAsync("/api/auth/login", new StringContent(JsonConvert.SerializeObject(new Models.Request.LoginRequest("user1", "Pa$$W0rd2")), Encoding.UTF8, "application/json"));
            loginResponse.EnsureSuccessStatusCode();
            var strLoginSuccessResponse1 = await loginResponse.Content.ReadAsStringAsync();
            dynamic loginResult1 = JObject.Parse(strLoginSuccessResponse1);
            Assert.NotNull(loginResult1.accessToken.token);
            Assert.Equal(7200,(int)loginResult1.accessToken.expiresIn);
            Assert.NotNull(loginResult1.refreshToken);
        }
    }
}