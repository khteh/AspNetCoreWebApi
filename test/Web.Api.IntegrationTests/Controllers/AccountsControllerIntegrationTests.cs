using Microsoft.Net.Http.Headers;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;
using static System.Net.Mime.MediaTypeNames;
namespace Web.Api.IntegrationTests.Controllers;

public class AccountsControllerIntegrationTests
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;
    public AccountsControllerIntegrationTests(ITestOutputHelper output, CustomWebApplicationFactory<Program> factory) => (_output, _client) = (output, factory.Client);
    [Fact]
    public async Task CanRegisterUserWithValidAccountDetails()
    {
        var httpResponse = await _client.PostAsync("/api/accounts/register", new StringContent(JsonSerializer.Serialize(new Models.Request.RegisterUserRequest("John", "Doe", "jdoe@gmail.com", "johndoe", "Pa$$word1")), Encoding.UTF8, Application.Json), TestContext.Current.CancellationToken);
        httpResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);
        string stringResponse = await httpResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.False(string.IsNullOrEmpty(stringResponse));
        JsonNode result = JsonNode.Parse(stringResponse);
        Assert.True((bool)result["success"]);
        Assert.Null(result["errors"]);
        Assert.False(string.IsNullOrEmpty((string)result["id"]));
    }
    [Fact]
    public async Task CanDeleteUserWithValidAccountDetails()
    {
        var httpResponse = await _client.DeleteAsync("/api/accounts/deleteme", TestContext.Current.CancellationToken);
        httpResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        string stringResponse = await httpResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.False(string.IsNullOrEmpty(stringResponse));
        JsonNode result = JsonNode.Parse(stringResponse);
        Assert.True((bool)result["success"]);
    }
    [Fact]
    public async Task CantRegisterUserWithInvalidAccountDetailsAndFailsFluentValidation()
    {
        var httpResponse = await _client.PostAsync("/api/accounts/register", new StringContent(JsonSerializer.Serialize(new Models.Request.RegisterUserRequest("John", "Doe", "me@email.com", string.Empty, "Pa$$word")), Encoding.UTF8, Application.Json), TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        string stringResponse = await httpResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.False(string.IsNullOrEmpty(stringResponse));
        JsonNode result = JsonNode.Parse(stringResponse);
        Assert.False((bool)result["success"]);
        Assert.NotNull(result["errors"]);
        Assert.Single(result["errors"].AsArray());
        foreach (JsonNode error in result["errors"].AsArray())
        {
            // HttpStatusCode.BadRequest.ToString(), "Invalid request input!"
            Assert.False(string.IsNullOrEmpty((string)error["code"]));
            Assert.Equal(HttpStatusCode.BadRequest.ToString(), (string)error["code"]);
            Assert.False(string.IsNullOrEmpty((string)error["description"]));
            Assert.Equal("Invalid request input!", (string)error["description"]);
        }
    }
    [Fact]
    public async Task CantRegisterUserWithInvalidEmailAndFailsFluentValidation()
    {
        var httpResponse = await _client.PostAsync("/api/accounts/register", new StringContent(JsonSerializer.Serialize(new Models.Request.RegisterUserRequest("John", "Doe", "email.com", string.Empty, "Pa$$word")), Encoding.UTF8, Application.Json), TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        string stringResponse = await httpResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.False(string.IsNullOrEmpty(stringResponse));
        JsonNode result = JsonNode.Parse(stringResponse);
        Assert.False((bool)result["success"]);
        Assert.NotNull(result["errors"]);
        Assert.Single(result["errors"].AsArray());
        foreach (JsonNode error in result["errors"].AsArray())
        {
            // HttpStatusCode.BadRequest.ToString(), "Invalid request input!"
            Assert.False(string.IsNullOrEmpty((string)error["code"]));
            Assert.Equal(HttpStatusCode.BadRequest.ToString(), (string)error["code"]);
            Assert.False(string.IsNullOrEmpty((string)error["description"]));
            Assert.Equal("Invalid request input!", (string)error["description"]);
        }
    }
    [Fact]
    public async Task CantDeleteUserWithInvalidAccountDetails()
    {
        var httpResponse = await _client.DeleteAsync("/api/accounts/DeleteMeNot", TestContext.Current.CancellationToken); // UserManager is NOT case sensitive!
        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        string stringResponse = await httpResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.False(string.IsNullOrEmpty(stringResponse));
        Assert.Contains("Invalid user!", stringResponse);
    }
    [Fact]
    public async Task CanFindById()
    {
        DateTimeOffset dt = DateTimeOffset.UtcNow;
        _client.DefaultRequestHeaders.Add(HeaderNames.IfModifiedSince, dt.ToString("R", CultureInfo.InvariantCulture));
        var httpResponse = await _client.GetAsync("/api/accounts/id/41532945-599e-4910-9599-0e7402017fbe", TestContext.Current.CancellationToken); // UserManager is NOT case sensitive!
        httpResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        string stringResponse = await httpResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.False(string.IsNullOrEmpty(stringResponse));
        JsonNode result = JsonNode.Parse(stringResponse);
        Assert.True((bool)result["success"]);
        string id = (string)result["id"];
        Assert.False(string.IsNullOrEmpty(id));
        Assert.Equal("41532945-599e-4910-9599-0e7402017fbe", id);
        /*
        httpResponse = await _client.GetAsync("/api/accounts/id/41532945-599e-4910-9599-0e7402017fbe", TestContext.Current.CancellationToken); // UserManager is NOT case sensitive!
        // This can only be tested once since _client is shared.
        httpResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NotModified, httpResponse.StatusCode); // https://github.com/dotnet/aspnetcore/issues/64763
        stringResponse = await httpResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Empty(stringResponse);
        */
    }
    [Fact]
    public async Task CanFindByUsername()
    {
        var httpResponse = await _client.GetAsync("/api/accounts/username/testuser", TestContext.Current.CancellationToken); // UserManager is NOT case sensitive!
        httpResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        string stringResponse = await httpResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.False(string.IsNullOrEmpty(stringResponse));
        JsonNode result = JsonNode.Parse(stringResponse);
        Assert.True((bool)result["success"]);
        string id = (string)result["id"];
        Assert.False(string.IsNullOrEmpty(id));
        Assert.Equal("41532945-599e-4910-9599-0e7402017fbe", id);
    }
    [Fact]
    public async Task CanFindByEmail()
    {
        //var httpResponse = await _client.GetAsync(WebUtility.UrlEncode("/api/accounts/email/testuser@email.com")); // UserManager is NOT case sensitive!
        var httpResponse = await _client.GetAsync("/api/accounts/email/testuser@email.com", TestContext.Current.CancellationToken); // UserManager is NOT case sensitive!
        httpResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        string stringResponse = await httpResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.False(string.IsNullOrEmpty(stringResponse));
        JsonNode result = JsonNode.Parse(stringResponse);
        Assert.True((bool)result["success"]);
        string id = (string)result["id"];
        Assert.False(string.IsNullOrEmpty(id));
        Assert.Equal("41532945-599e-4910-9599-0e7402017fbe", id);
    }
    [Fact]
    public async Task CanChangePasswordWithValidAccountDetails()
    {
        // Create User
        var httpResponse = await _client.PostAsync("/api/accounts/register", new StringContent(JsonSerializer.Serialize(new Models.Request.RegisterUserRequest("FirstName", "LastName", "user@gmail.com", "user1", "Pa$$word1")), Encoding.UTF8, Application.Json), TestContext.Current.CancellationToken);
        httpResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);
        JsonNode result = JsonNode.Parse(await httpResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        Assert.True((bool)result["success"]);
        string id = (string)result["id"];
        Assert.False(string.IsNullOrEmpty(id));

        // Login
        Models.Request.LogInRequest loginRequest = new Models.Request.LogInRequest("user1", "Pa$$word1");
        var loginResponse = await _client.PostAsync("/api/auth/login", new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, Application.Json), TestContext.Current.CancellationToken);
        loginResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        string strLoginResponse = await loginResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        if (!loginResponse.IsSuccessStatusCode)
            _output.WriteLine($"{nameof(CanChangePasswordWithValidAccountDetails)} failed with error message:  {strLoginResponse}");
        string strLoginSuccessResponse = await loginResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.False(string.IsNullOrEmpty(strLoginSuccessResponse));
        Models.Response.LogInResponse loginResult = Serialization.JsonSerializer.DeSerializeObject<Models.Response.LogInResponse>(strLoginSuccessResponse);
        Assert.NotNull(loginResult);
        Assert.True(loginResult.Success);
        Assert.Null(loginResult.Errors);
        Assert.NotNull(loginResult.AccessToken);
        Assert.NotNull(loginResult.AccessToken.Token);
        Assert.Equal(7200, (int)loginResult.AccessToken.ExpiresIn);
        Assert.NotNull(loginResult.RefreshToken);

        // Change Password
        var pwdResponse = await _client.PostAsync("/api/accounts/changepassword", new StringContent(JsonSerializer.Serialize(new Models.Request.ChangePasswordRequest(id, "Pa$$word1", "Pa$$word2")), Encoding.UTF8, Application.Json), TestContext.Current.CancellationToken);
        pwdResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, pwdResponse.StatusCode);
        string strPwdResponse = await pwdResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.False(string.IsNullOrEmpty(strPwdResponse));
        Models.Response.ChangePasswordResponse pwdResponse1 = Serialization.JsonSerializer.DeSerializeObject<Models.Response.ChangePasswordResponse>(strPwdResponse);
        Assert.True(pwdResponse1.Success);
        Assert.Null(pwdResponse1.Errors);

        // Should fail login with previous password
        var loginFailResponse = await _client.PostAsync("/api/auth/login", new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, Application.Json), TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Unauthorized, loginFailResponse.StatusCode);
        string strLoginFailResponse = await loginFailResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.False(string.IsNullOrEmpty(strLoginFailResponse));
        Models.Response.LogInResponse response = Serialization.JsonSerializer.DeSerializeObject<Models.Response.LogInResponse>(strLoginFailResponse);
        Assert.NotNull(response);
        Assert.NotNull(response.Errors);
        Assert.NotEmpty(response.Errors);
        Assert.Contains(HttpStatusCode.Unauthorized.ToString(), response.Errors.First().Code);
        Assert.Contains("Invalid username", response.Errors.First().Description);

        // Login
        var loginSuccessResponse = await _client.PostAsync("/api/auth/login", new StringContent(JsonSerializer.Serialize(loginRequest with { Password = "Pa$$word2" }), Encoding.UTF8, Application.Json), TestContext.Current.CancellationToken);
        loginSuccessResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, loginSuccessResponse.StatusCode);
        string strLoginSuccessResponse1 = await loginSuccessResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.False(string.IsNullOrEmpty(strLoginSuccessResponse1));
        JsonNode loginResult1 = JsonNode.Parse(strLoginSuccessResponse1);
        Assert.NotNull(loginResult1["accessToken"]);
        Assert.False(string.IsNullOrEmpty((string)loginResult1["accessToken"]["token"]));
        Assert.Equal(7200, (int)loginResult1["accessToken"]["expiresIn"]);
        Assert.False(string.IsNullOrEmpty((string)loginResult1["refreshToken"]));
    }
    [Fact]
    public async Task CanResetPasswordWithValidAccountDetails()
    {
        // Create User
        var httpResponse = await _client.PostAsync("/api/accounts/register", new StringContent(JsonSerializer.Serialize(new Models.Request.RegisterUserRequest("FirstName", "LastName", "user1@gmail.com", "user2", "Pa$$word1")), Encoding.UTF8, Application.Json), TestContext.Current.CancellationToken);
        httpResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);
        JsonNode result = JsonNode.Parse(await httpResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        Assert.True((bool)result["success"]);
        string id = (string)result["id"];
        Assert.False(string.IsNullOrEmpty(id));

        // Login
        Models.Request.LogInRequest loginRequest = new Models.Request.LogInRequest("user2", "Pa$$word1");
        var loginResponse = await _client.PostAsync("/api/auth/login", new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, Application.Json), TestContext.Current.CancellationToken);
        loginResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        string strLoginSuccessResponse = await loginResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.False(string.IsNullOrEmpty(strLoginSuccessResponse));
        Models.Response.LogInResponse loginResult = Serialization.JsonSerializer.DeSerializeObject<Models.Response.LogInResponse>(strLoginSuccessResponse);
        Assert.NotNull(loginResult);
        Assert.True(loginResult.Success);
        Assert.Null(loginResult.Errors);
        Assert.NotNull(loginResult.AccessToken);
        Assert.Equal(7200, (int)loginResult.AccessToken.ExpiresIn);
        Assert.False(string.IsNullOrEmpty(loginResult.AccessToken.Token));
        Assert.False(string.IsNullOrEmpty(loginResult.RefreshToken));

        // Change Password
        var pwdResponse = await _client.PostAsync("/api/accounts/resetpassword", new StringContent(JsonSerializer.Serialize(new Models.Request.ResetPasswordRequest(id, string.Empty, "Pa$$word2", string.Empty)), Encoding.UTF8, Application.Json), TestContext.Current.CancellationToken);
        pwdResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, pwdResponse.StatusCode);
        string strPwdResponse = await pwdResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.False(string.IsNullOrEmpty(strPwdResponse));
        Models.Response.ResetPasswordResponse pwdResponse1 = Serialization.JsonSerializer.DeSerializeObject<Models.Response.ResetPasswordResponse>(strPwdResponse);
        Assert.True(pwdResponse1.Success);
        Assert.Equal(HttpStatusCode.OK, pwdResponse.StatusCode);
        Assert.Null(pwdResponse1.Errors);

        // Should fail login with previous password
        var loginFailResponse = await _client.PostAsync("/api/auth/login", new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, Application.Json), TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Unauthorized, loginFailResponse.StatusCode);
        string strLoginFailResponse = await loginFailResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.False(string.IsNullOrEmpty(strLoginFailResponse));
        Models.Response.LogInResponse response = Serialization.JsonSerializer.DeSerializeObject<Models.Response.LogInResponse>(strLoginFailResponse);
        Assert.NotNull(response);
        Assert.NotNull(response.Errors);
        Assert.NotEmpty(response.Errors);
        Assert.Contains(HttpStatusCode.Unauthorized.ToString(), response.Errors.First().Code);
        Assert.Contains("Invalid username", response.Errors.First().Description);

        // Login
        var loginSuccessResponse = await _client.PostAsync("/api/auth/login", new StringContent(JsonSerializer.Serialize(loginRequest with { Password = "Pa$$word2" }), Encoding.UTF8, Application.Json), TestContext.Current.CancellationToken);
        loginSuccessResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, loginSuccessResponse.StatusCode);
        string strLoginSuccessResponse1 = await loginSuccessResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.False(string.IsNullOrEmpty(strLoginSuccessResponse1));
        JsonNode loginResult1 = JsonNode.Parse(strLoginSuccessResponse1);
        Assert.NotNull(loginResult1["accessToken"]);
        Assert.False(string.IsNullOrEmpty((string)loginResult1["accessToken"]["token"]));
        Assert.Equal(7200, (int)loginResult1["accessToken"]["expiresIn"]);
        Assert.False(string.IsNullOrEmpty((string)loginResult1["refreshToken"]));
    }
}