using System.Reflection;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Web.Api.Core.DTO.UseCaseRequests;
using Xunit;
using System.Security.Cryptography.X509Certificates;
using Web.Api.Core.Accounts;
using Web.Api.Core.Accounts;
using Grpc.Net.Client;

namespace Web.Api.IntegrationTests.Services
{
    #if false
    public class AccountsServiceIntegrationTests : IClassFixture<CustomGrpcServerFactory<Accounts.AccountsClient, Startup>>
    {
        private Accounts.AccountsClient _client;
        public AccountsServiceIntegrationTests(CustomGrpcServerFactory<Accounts.AccountsClient, Startup> factory)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;
            _client = factory.Client;
            // The port number(5001) must match the port of the gRPC server.
        }

        [Fact]
        public async Task CanRegisterUserWithValidAccountDetails()
        {
            // Act
            RegisterUserResponse response = await _client.RegisterAsync(new RegisterUserRequest() {
                FirstName = "John",
                LastName = "Doe",
                Email = "jdoe@gmail.com",
                UserName = "johndoe",
                Password = "P@$$w0rd1"
            });//.ResponseAsync.DefaultTimeout();

            // Assert
            //Assert.AreEqual(deadline, options.Deadline);
            //Assert.AreEqual(cancellationToken, options.CancellationToken);
            Assert.NotNull(response);
            Assert.NotNull(response.Response);
            Assert.True(response.Response.Success);
            Assert.Null(response.Response.Errors);
            Assert.False(string.IsNullOrEmpty(response.Id));
        }
        [Fact]
        public async Task CanDeleteUserWithValidAccountDetails()
        {
            DeleteUserResponse response = await _client.DeleteAsync(new StringInputParameter() { Value = "deleteme"});
            Assert.NotNull(response);
            Assert.NotNull(response.Response);
            Assert.True(response.Response.Success);
            Assert.Null(response.Response.Errors);
            Assert.False(string.IsNullOrEmpty(response.Id));
        }

        [Fact]
        public async Task CantRegisterUserWithInvalidAccountDetails()
        {
            // Act
            RegisterUserResponse response = await _client.RegisterAsync(new RegisterUserRequest() {
                FirstName = "John",
                LastName = "Doe",
                Email = string.Empty,
                UserName = "johndoe",
                Password = "P@$$w0rd1"
            });//.ResponseAsync.DefaultTimeout();

            // Assert
            //Assert.AreEqual(deadline, options.Deadline);
            //Assert.AreEqual(cancellationToken, options.CancellationToken);
            Assert.NotNull(response);
            Assert.NotNull(response.Response);
            Assert.False(response.Response.Success);
            Assert.NotNull(response.Response.Errors);
            Assert.Single(response.Response.Errors);
            Assert.True(string.IsNullOrEmpty(response.Id));
            Assert.Equal(HttpStatusCode.BadRequest.ToString(), response.Response.Errors.First().Code);
            Assert.Equal("'Email' is not a valid email address.", response.Response.Errors.First().Description);
        }
        [Fact]
        public async Task CantDeleteUserWithInvalidAccountDetails()
        {
            DeleteUserResponse response = await _client.DeleteAsync(new StringInputParameter() { Value = "DeleteMeNot"});
            Assert.NotNull(response);
            Assert.NotNull(response.Response);
            Assert.False(response.Response.Success);
            Assert.NotNull(response.Response.Errors);
            Assert.Single(response.Response.Errors);
            Assert.True(string.IsNullOrEmpty(response.Id));
            Assert.Equal(HttpStatusCode.BadRequest.ToString(), response.Response.Errors.First().Code);
            Assert.Equal("Invalid user!", response.Response.Errors.First().Description);
        }
        [Fact]
        public async Task CanFindById()
        {
            FindUserResponse response = await _client.FindByIdAsync(new StringInputParameter() { Value = "41532945-599e-4910-9599-0e7402017fbe"});
            Assert.NotNull(response);
            Assert.NotNull(response.Response);
            Assert.True(response.Response.Success);
            Assert.Null(response.Response.Errors);
            Assert.False(string.IsNullOrEmpty(response.Id));
            Assert.Equal("41532945-599e-4910-9599-0e7402017fbe", response.Id);
        }
        [Fact]
        public async Task CanFindByUsername()
        {
            FindUserResponse response = await _client.FindByUserNameAsync(new StringInputParameter() { Value = "mickeymouse"}); // UserManager is NOT case sensitive!
            Assert.NotNull(response);
            Assert.NotNull(response.Response);
            Assert.True(response.Response.Success);
            Assert.Null(response.Response.Errors);
            Assert.False(string.IsNullOrEmpty(response.Id));
            Assert.Equal("41532945-599e-4910-9599-0e7402017fbe", response.Id);
        }
        [Fact]
        public async Task CanFindByEmail()
        {
            //var httpResponse = await _client.GetAsync(WebUtility.UrlEncode("/accounts/email/mickey@mouse.com")); // UserManager is NOT case sensitive!
            FindUserResponse response = await _client.FindByEmailAsync(new StringInputParameter() { Value = "mickey@mouse.com"}); // UserManager is NOT case sensitive!
            Assert.NotNull(response);
            Assert.NotNull(response.Response);
            Assert.True(response.Response.Success);
            Assert.Null(response.Response.Errors);
            Assert.False(string.IsNullOrEmpty(response.Id));
            Assert.Equal("41532945-599e-4910-9599-0e7402017fbe", response.Id);
        }
        [Fact]
        public async Task CanChangePasswordWithValidAccountDetails()
        {
            // Create User
            RegisterUserResponse response = await _client.RegisterAsync(new RegisterUserRequest() {
                FirstName = "FirstName",
                LastName = "LastName",
                Email = "user@gmail.com",
                UserName = "user1",
                Password = "P@$$w0rd1"
            });//.ResponseAsync.DefaultTimeout();
            Assert.NotNull(response);
            Assert.NotNull(response.Response);
            Assert.True(response.Response.Success);
            Assert.Null(response.Response.Errors);
            Assert.False(string.IsNullOrEmpty(response.Id));

            // Login
            var loginResponse = await _client.PostAsync("/auth/login", new StringContent(JsonConvert.SerializeObject(new Models.Request.LoginRequest("user1", "Pa$$word1")), Encoding.UTF8, "application/json"));
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
            var pwdResponse = await _client.PostAsync("/accounts/changepassword", new StringContent(JsonConvert.SerializeObject(new Models.Request.ChangePasswordRequest((string)result.id, "Pa$$word1", "Pa$$word2")), Encoding.UTF8, "application/json"));
            pwdResponse.EnsureSuccessStatusCode();
            var strPwdResponse = await pwdResponse.Content.ReadAsStringAsync();
            //dynamic pwdResult = JObject.Parse(await pwdResponse.Content.ReadAsStringAsync());
            Models.Response.ChangePasswordResponse pwdResponse1 = Serialization.JsonSerializer.DeSerializeObject<Models.Response.ChangePasswordResponse>(strPwdResponse);
            Assert.True(pwdResponse1.Success);
            Assert.Equal(HttpStatusCode.OK, pwdResponse.StatusCode);
            Assert.Null(pwdResponse1.Errors);

            // Should fail login with previous password
            var loginFailResponse = await _client.PostAsync("/auth/login", new StringContent(JsonConvert.SerializeObject(new Models.Request.LoginRequest("user1", "Pa$$W0rd1")), Encoding.UTF8, "application/json"));
            var strLoginFailResponse = await loginFailResponse.Content.ReadAsStringAsync();
            Models.Response.LoginResponse response = Serialization.JsonSerializer.DeSerializeObject<Models.Response.LoginResponse>(strLoginFailResponse);
            Assert.NotNull(response);
            Assert.NotNull(response.Errors);
            Assert.NotEmpty(response.Errors);
            Assert.Contains(HttpStatusCode.Unauthorized.ToString(), response.Errors.First().Code);
            Assert.Contains("Invalid username", response.Errors.First().Description);

            // Login
            var loginSuccessResponse = await _client.PostAsync("/auth/login", new StringContent(JsonConvert.SerializeObject(new Models.Request.LoginRequest("user1", "Pa$$W0rd2")), Encoding.UTF8, "application/json"));
            loginResponse.EnsureSuccessStatusCode();
            var strLoginSuccessResponse1 = await loginResponse.Content.ReadAsStringAsync();
            dynamic loginResult1 = JObject.Parse(strLoginSuccessResponse1);
            Assert.NotNull(loginResult1.accessToken.token);
            Assert.Equal(7200,(int)loginResult1.accessToken.expiresIn);
            Assert.NotNull(loginResult1.refreshToken);
        }
        [Fact]
        public async Task CanResetPasswordWithValidAccountDetails()
        {
            // Create User
            var httpResponse = await _client.PostAsync("/accounts/register", new StringContent(JsonConvert.SerializeObject(new Models.Request.RegisterUserRequest("FirstName", "LastName", "user1@gmail.com", "user2", "Pa$$word1")), Encoding.UTF8, "application/json"));
            httpResponse.EnsureSuccessStatusCode();
            dynamic result = JObject.Parse(await httpResponse.Content.ReadAsStringAsync());
            Assert.True((bool) result.success);
            Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);
            Assert.False(string.IsNullOrEmpty((string)result.id));

            // Login
            var loginResponse = await _client.PostAsync("/auth/login", new StringContent(JsonConvert.SerializeObject(new Models.Request.LoginRequest("user2", "Pa$$word1")), Encoding.UTF8, "application/json"));
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
            var pwdResponse = await _client.PostAsync("/accounts/resetpassword", new StringContent(JsonConvert.SerializeObject(new Models.Request.ResetPasswordRequest((string)result.id, "Pa$$word2")), Encoding.UTF8, "application/json"));
            pwdResponse.EnsureSuccessStatusCode();
            var strPwdResponse = await pwdResponse.Content.ReadAsStringAsync();
            //dynamic pwdResult = JObject.Parse(await pwdResponse.Content.ReadAsStringAsync());
            Models.Response.ResetPasswordResponse pwdResponse1 = Serialization.JsonSerializer.DeSerializeObject<Models.Response.ResetPasswordResponse>(strPwdResponse);
            Assert.True(pwdResponse1.Success);
            Assert.Equal(HttpStatusCode.OK, pwdResponse.StatusCode);
            Assert.Null(pwdResponse1.Errors);

            // Should fail login with previous password
            var loginFailResponse = await _client.PostAsync("/auth/login", new StringContent(JsonConvert.SerializeObject(new Models.Request.LoginRequest("user2", "Pa$$word1")), Encoding.UTF8, "application/json"));
            var strLoginFailResponse = await loginFailResponse.Content.ReadAsStringAsync();
            Models.Response.LoginResponse response = Serialization.JsonSerializer.DeSerializeObject<Models.Response.LoginResponse>(strLoginFailResponse);
            Assert.NotNull(response);
            Assert.NotNull(response.Errors);
            Assert.NotEmpty(response.Errors);
            Assert.Contains(HttpStatusCode.Unauthorized.ToString(), response.Errors.First().Code);
            Assert.Contains("Invalid username", response.Errors.First().Description);

            // Login
            var loginSuccessResponse = await _client.PostAsync("/auth/login", new StringContent(JsonConvert.SerializeObject(new Models.Request.LoginRequest("user2", "Pa$$word2")), Encoding.UTF8, "application/json"));
            loginResponse.EnsureSuccessStatusCode();
            var strLoginSuccessResponse1 = await loginResponse.Content.ReadAsStringAsync();
            dynamic loginResult1 = JObject.Parse(strLoginSuccessResponse1);
            Assert.NotNull(loginResult1.accessToken.token);
            Assert.Equal(7200,(int)loginResult1.accessToken.expiresIn);
            Assert.NotNull(loginResult1.refreshToken);
        }
    }
    #endif
}