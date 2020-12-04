using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Web.Api.IntegrationTests.Controllers
{
    public class AccountsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public AccountsControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory) => _client = factory.CreateClient();

        [Fact]
        public async Task CanRegisterUserWithValidAccountDetails()
        {
            var httpResponse = await _client.PostAsync("/api/accounts/register", new StringContent(System.Text.Json.JsonSerializer.Serialize(new Models.Request.RegisterUserRequest("John", "Doe", "jdoe@gmail.com", "johndoe", "Pa$$word1")), Encoding.UTF8, "application/json"));
            httpResponse.EnsureSuccessStatusCode();
            string stringResponse = await httpResponse.Content.ReadAsStringAsync();
            JsonDocument result = JsonDocument.Parse(stringResponse);
            Assert.True(result.RootElement.GetProperty("success").GetBoolean());
            Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);
            Assert.False(result.RootElement.TryGetProperty("errors", out _));
            Assert.False(string.IsNullOrEmpty(result.RootElement.GetProperty("id").GetString()));
        }
        [Fact]
        public async Task CanDeleteUserWithValidAccountDetails()
        {
            var httpResponse = await _client.DeleteAsync("/api/accounts/deleteme");
            httpResponse.EnsureSuccessStatusCode();
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            JsonDocument result = JsonDocument.Parse(stringResponse);
            Assert.True(result.RootElement.GetProperty("success").GetBoolean());
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        }

        [Fact]
        public async Task CantRegisterUserWithInvalidAccountDetailsAndFailsFluentValidation()
        {
            var httpResponse = await _client.PostAsync("/api/accounts/register", new StringContent(System.Text.Json.JsonSerializer.Serialize(new Models.Request.RegisterUserRequest("John", "Doe", string.Empty, string.Empty, "Pa$$word")), Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            JsonDocument result = JsonDocument.Parse(stringResponse);
            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.RootElement.GetProperty("status").GetInt32());
            Assert.Equal("One or more validation errors occurred.", (string)result.RootElement.GetProperty("title").GetString());
            Assert.True(result.RootElement.TryGetProperty("errors", out JsonElement error));
            Assert.True(error.TryGetProperty("Email", out JsonElement emails));
            Assert.NotEqual(0, emails.GetArrayLength());
            Assert.Equal(1, emails.GetArrayLength());
            foreach (JsonElement email in emails.EnumerateArray())
            {
                Assert.False(string.IsNullOrEmpty(email.GetString()));
                Assert.Equal("'Email' is not a valid email address.", email.GetString());
            }
            Assert.True(error.TryGetProperty("UserName", out JsonElement userNames));
            Assert.NotEqual(0, userNames.GetArrayLength());
            Assert.Equal(1, userNames.GetArrayLength());
            foreach (JsonElement userName in userNames.EnumerateArray())
            {
                Assert.False(string.IsNullOrEmpty(userName.GetString()));
                Assert.Equal("'User Name' must be between 3 and 255 characters. You entered 0 characters.", userName.GetString());
            }
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
            JsonDocument result = JsonDocument.Parse(stringResponse);
            Assert.True(result.RootElement.GetProperty("success").GetBoolean());
            string id = result.RootElement.GetProperty("id").GetString();
            Assert.False(string.IsNullOrEmpty(id));
            Assert.Equal("41532945-599e-4910-9599-0e7402017fbe", id);
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        }
        [Fact]
        public async Task CanFindByUsername()
        {
            var httpResponse = await _client.GetAsync("/api/accounts/username/mickeymouse"); // UserManager is NOT case sensitive!
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            JsonDocument result = JsonDocument.Parse(stringResponse);
            Assert.True(result.RootElement.GetProperty("success").GetBoolean());
            string id = result.RootElement.GetProperty("id").GetString();
            Assert.False(string.IsNullOrEmpty(id));
            Assert.Equal("41532945-599e-4910-9599-0e7402017fbe", id);
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        }
        [Fact]
        public async Task CanFindByEmail()
        {
            //var httpResponse = await _client.GetAsync(WebUtility.UrlEncode("/api/accounts/email/mickey@mouse.com")); // UserManager is NOT case sensitive!
            var httpResponse = await _client.GetAsync("/api/accounts/email/mickey@mouse.com"); // UserManager is NOT case sensitive!
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            JsonDocument result = JsonDocument.Parse(stringResponse);
            Assert.True(result.RootElement.GetProperty("success").GetBoolean());
            string id = result.RootElement.GetProperty("id").GetString();
            Assert.False(string.IsNullOrEmpty(id));
            Assert.Equal("41532945-599e-4910-9599-0e7402017fbe", id);
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        }
        [Fact]
        public async Task CanChangePasswordWithValidAccountDetails()
        {
            // Create User
            var httpResponse = await _client.PostAsync("/api/accounts/register", new StringContent(System.Text.Json.JsonSerializer.Serialize(new Models.Request.RegisterUserRequest("FirstName", "LastName", "user@gmail.com", "user1", "Pa$$word1")), Encoding.UTF8, "application/json"));
            httpResponse.EnsureSuccessStatusCode();
            JsonDocument result = JsonDocument.Parse(await httpResponse.Content.ReadAsStringAsync());
            Assert.True(result.RootElement.GetProperty("success").GetBoolean());
            Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);
            string id = result.RootElement.GetProperty("id").GetString();
            Assert.False(string.IsNullOrEmpty(id));

            // Login
            Models.Request.LoginRequest loginRequest = new Models.Request.LoginRequest("user1", "Pa$$word1");
            var loginResponse = await _client.PostAsync("/api/auth/login", new StringContent(System.Text.Json.JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json"));
            loginResponse.EnsureSuccessStatusCode();
            var strLoginSuccessResponse = await loginResponse.Content.ReadAsStringAsync();
            Models.Response.LoginResponse loginResult = Serialization.JsonSerializer.DeSerializeObject<Models.Response.LoginResponse>(strLoginSuccessResponse);
            Assert.NotNull(loginResult);
            Assert.True(loginResult.Success);
            Assert.Null(loginResult.Errors);
            Assert.NotNull(loginResult.AccessToken.Token);
            Assert.Equal(7200,(int)loginResult.AccessToken.ExpiresIn);
            Assert.NotNull(loginResult.RefreshToken);

            // Change Password
            var pwdResponse = await _client.PostAsync("/api/accounts/changepassword", new StringContent(System.Text.Json.JsonSerializer.Serialize(new Models.Request.ChangePasswordRequest(id, "Pa$$word1", "Pa$$word2")), Encoding.UTF8, "application/json"));
            pwdResponse.EnsureSuccessStatusCode();
            var strPwdResponse = await pwdResponse.Content.ReadAsStringAsync();
            //dynamic pwdResult = JsonDocument.Parse(await pwdResponse.Content.ReadAsStringAsync());
            Models.Response.ChangePasswordResponse pwdResponse1 = Serialization.JsonSerializer.DeSerializeObject<Models.Response.ChangePasswordResponse>(strPwdResponse);
            Assert.True(pwdResponse1.Success);
            Assert.Equal(HttpStatusCode.OK, pwdResponse.StatusCode);
            Assert.Null(pwdResponse1.Errors);

            // Should fail login with previous password
            var loginFailResponse = await _client.PostAsync("/api/auth/login", new StringContent(System.Text.Json.JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json"));
            var strLoginFailResponse = await loginFailResponse.Content.ReadAsStringAsync();
            Models.Response.LoginResponse response = Serialization.JsonSerializer.DeSerializeObject<Models.Response.LoginResponse>(strLoginFailResponse);
            Assert.NotNull(response);
            Assert.NotNull(response.Errors);
            Assert.NotEmpty(response.Errors);
            Assert.Contains(HttpStatusCode.Unauthorized.ToString(), response.Errors.First().Code);
            Assert.Contains("Invalid username", response.Errors.First().Description);

            // Login
            var loginSuccessResponse = await _client.PostAsync("/api/auth/login", new StringContent(System.Text.Json.JsonSerializer.Serialize(loginRequest with { Password = "Pa$$word2" }), Encoding.UTF8, "application/json"));
            loginSuccessResponse.EnsureSuccessStatusCode();
            var strLoginSuccessResponse1 = await loginSuccessResponse.Content.ReadAsStringAsync();
            JsonDocument loginResult1 = JsonDocument.Parse(strLoginSuccessResponse1);
            Assert.True(loginResult1.RootElement.TryGetProperty("accessToken", out JsonElement accessToken));
            Assert.True(accessToken.TryGetProperty("token", out JsonElement token));
            Assert.True(accessToken.TryGetProperty("expiresIn", out JsonElement expiry));
            Assert.True(loginResult1.RootElement.TryGetProperty("refreshToken", out JsonElement refreshToken));
            Assert.False(string.IsNullOrEmpty(token.GetString()));
            Assert.Equal(7200, expiry.GetInt32());
            Assert.False(string.IsNullOrEmpty(refreshToken.GetString()));
        }
        [Fact]
        public async Task CanResetPasswordWithValidAccountDetails()
        {
            // Create User
            var httpResponse = await _client.PostAsync("/api/accounts/register", new StringContent(System.Text.Json.JsonSerializer.Serialize(new Models.Request.RegisterUserRequest("FirstName", "LastName", "user1@gmail.com", "user2", "Pa$$word1")), Encoding.UTF8, "application/json"));
            httpResponse.EnsureSuccessStatusCode();
            JsonDocument result = JsonDocument.Parse(await httpResponse.Content.ReadAsStringAsync());
            Assert.True(result.RootElement.GetProperty("success").GetBoolean());
            Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);
            string id = result.RootElement.GetProperty("id").GetString();
            Assert.False(string.IsNullOrEmpty(id));

            // Login
            Models.Request.LoginRequest loginRequest = new Models.Request.LoginRequest("user2", "Pa$$word1");
            var loginResponse = await _client.PostAsync("/api/auth/login", new StringContent(System.Text.Json.JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json"));
            loginResponse.EnsureSuccessStatusCode();
            var strLoginSuccessResponse = await loginResponse.Content.ReadAsStringAsync();
            Models.Response.LoginResponse loginResult = Serialization.JsonSerializer.DeSerializeObject<Models.Response.LoginResponse>(strLoginSuccessResponse);
            Assert.NotNull(loginResult);
            Assert.True(loginResult.Success);
            Assert.Null(loginResult.Errors);
            Assert.NotNull(loginResult.AccessToken);
            Assert.Equal(7200,(int)loginResult.AccessToken.ExpiresIn);
            Assert.False(string.IsNullOrEmpty(loginResult.AccessToken.Token));
            Assert.False(string.IsNullOrEmpty(loginResult.RefreshToken));

            // Change Password
            var pwdResponse = await _client.PostAsync("/api/accounts/resetpassword", new StringContent(System.Text.Json.JsonSerializer.Serialize(new Models.Request.ResetPasswordRequest(id, "Pa$$word2")), Encoding.UTF8, "application/json"));
            pwdResponse.EnsureSuccessStatusCode();
            var strPwdResponse = await pwdResponse.Content.ReadAsStringAsync();
            //dynamic pwdResult = JsonDocument.Parse(await pwdResponse.Content.ReadAsStringAsync());
            Models.Response.ResetPasswordResponse pwdResponse1 = Serialization.JsonSerializer.DeSerializeObject<Models.Response.ResetPasswordResponse>(strPwdResponse);
            Assert.True(pwdResponse1.Success);
            Assert.Equal(HttpStatusCode.OK, pwdResponse.StatusCode);
            Assert.Null(pwdResponse1.Errors);

            // Should fail login with previous password
            var loginFailResponse = await _client.PostAsync("/api/auth/login", new StringContent(System.Text.Json.JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json"));
            var strLoginFailResponse = await loginFailResponse.Content.ReadAsStringAsync();
            Models.Response.LoginResponse response = Serialization.JsonSerializer.DeSerializeObject<Models.Response.LoginResponse>(strLoginFailResponse);
            Assert.NotNull(response);
            Assert.NotNull(response.Errors);
            Assert.NotEmpty(response.Errors);
            Assert.Contains(HttpStatusCode.Unauthorized.ToString(), response.Errors.First().Code);
            Assert.Contains("Invalid username", response.Errors.First().Description);

            // Login
            var loginSuccessResponse = await _client.PostAsync("/api/auth/login", new StringContent(System.Text.Json.JsonSerializer.Serialize(loginRequest with { Password = "Pa$$word2" }), Encoding.UTF8, "application/json"));
            loginSuccessResponse.EnsureSuccessStatusCode();
            var strLoginSuccessResponse1 = await loginSuccessResponse.Content.ReadAsStringAsync();
            JsonDocument loginResult1 = JsonDocument.Parse(strLoginSuccessResponse1);
            Assert.True(loginResult1.RootElement.TryGetProperty("accessToken", out JsonElement accessToken));
            Assert.True(accessToken.TryGetProperty("token", out JsonElement token));
            Assert.True(accessToken.TryGetProperty("expiresIn", out JsonElement expiry));
            Assert.True(loginResult1.RootElement.TryGetProperty("refreshToken", out JsonElement refreshToken));
            Assert.False(string.IsNullOrEmpty(token.GetString()));
            Assert.Equal(7200, expiry.GetInt32());
            Assert.False(string.IsNullOrEmpty(refreshToken.GetString()));
        }
    }
}