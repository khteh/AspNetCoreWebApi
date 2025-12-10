using Elastic.CommonSchema;
using System;
using System.Linq;
using System.Net;
using System.Threading.Channels;
using System.Threading.Tasks;
using Web.Api.Identity.Accounts;
using Web.Api.Identity.Auth;
using Xunit;
using static Web.Api.Identity.Accounts.Accounts;
using static Web.Api.Identity.Auth.Auth;
namespace Web.Api.IntegrationTests.Services;

//[Collection("GRPC Test Collection")]
public class AccountsServiceIntegrationTests// : IntegrationTestBase
{
    private readonly ITestOutputHelper _output;
    private readonly AccountsClient _accountsClient;
    private readonly AuthClient _authClient;
    //public AccountsServiceIntegrationTests(GrpcTestFixture<Program> fixture, ITestOutputHelper output) : base(fixture, output) { }
    public AccountsServiceIntegrationTests(ITestOutputHelper output, CustomWebApplicationFactory<Program> factory)
    {
        _output = output;
        _accountsClient = new AccountsClient(factory.GrpcChannel);
        _authClient = new AuthClient(factory.GrpcChannel);
    }
    [Fact]
    public async Task CanRegisterUserWithValidAccountDetails()
    {
        // Arrange
        Assert.NotNull(_accountsClient);
        // Act
        RegisterUserResponse response = await _accountsClient.RegisterAsync(new RegisterUserRequest()
        {
            FirstName = "John Grpc",
            LastName = "Doe",
            Email = "jdoegrpc@gmail.com",
            UserName = "johndoegrpc",
            Password = "P@$$w0rd"
        }, null, DateTime.UtcNow.AddSeconds(10), TestContext.Current.CancellationToken);//.ResponseAsync.DefaultTimeout();

        // Assert
        //Assert.AreEqual(deadline, options.Deadline);
        //Assert.AreEqual(cancellationToken, options.CancellationToken);
        Assert.NotNull(response);
        Assert.NotNull(response.Response);
        Assert.True(response.Response.Success);
        Assert.Empty(response.Response.Errors);
        Assert.False(string.IsNullOrEmpty(response.Id));
    }
    [Fact]
    public async Task CanDeleteUserWithValidAccountDetails()
    {
        Assert.NotNull(_accountsClient);
        DeleteUserResponse response = await _accountsClient.DeleteAsync(new StringInputParameter() { Value = "deletemegrpc" }, null, DateTime.UtcNow.AddSeconds(10), TestContext.Current.CancellationToken);
        Assert.NotNull(response);
        Assert.NotNull(response.Response);
        Assert.True(response.Response.Success);
        Assert.Empty(response.Response.Errors);
        Assert.False(string.IsNullOrEmpty(response.Id));
    }
    [Fact]
    public async Task CantRegisterUserWithInvalidAccountDetails()
    {
        Assert.NotNull(_accountsClient);
        // Act
        RegisterUserResponse response = await _accountsClient.RegisterAsync(new RegisterUserRequest()
        {
            FirstName = "John Grpc",
            LastName = "Doe",
            Email = string.Empty,
            UserName = string.Empty,
            Password = "P@$$w0rd"
        }, null, DateTime.UtcNow.AddSeconds(10), TestContext.Current.CancellationToken);//.ResponseAsync.DefaultTimeout();

        // Assert
        //Assert.AreEqual(deadline, options.Deadline);
        //Assert.AreEqual(cancellationToken, options.CancellationToken);
        Assert.NotNull(response);
        Assert.NotNull(response.Response);
        Assert.False(response.Response.Success);
        Assert.Single(response.Response.Errors);
        Assert.True(string.IsNullOrEmpty(response.Id));
        Assert.Equal("InvalidUserName", response.Response.Errors.First().Code);
        //Assert.Equal("'Email' is not a valid email address.", response.Response.Errors.First().Description);
    }
    [Fact]
    public async Task CantDeleteUserWithInvalidAccountDetails()
    {
        Assert.NotNull(_accountsClient);
        DeleteUserResponse response = await _accountsClient.DeleteAsync(new StringInputParameter() { Value = "DeleteMeNot" }, null, DateTime.UtcNow.AddSeconds(10), TestContext.Current.CancellationToken);
        Assert.NotNull(response);
        Assert.NotNull(response.Response);
        Assert.False(response.Response.Success);
        Assert.Single(response.Response.Errors);
        Assert.True(string.IsNullOrEmpty(response.Id));
        Assert.Equal(HttpStatusCode.BadRequest.ToString(), response.Response.Errors.First().Code);
        Assert.Equal("Invalid user!", response.Response.Errors.First().Description);
    }
    [Theory]
    [InlineData("CE73A87D-0AA6-4191-B65B-6B49F333E316")]
    [InlineData("3ABE9D63-777C-4865-8FA0-A53A657313D5")]
    public async Task CanFindById(string id)
    {
        Assert.NotNull(_accountsClient);
        FindUserResponse response = await _accountsClient.FindByIdAsync(new StringInputParameter() { Value = id }, null, DateTime.UtcNow.AddSeconds(10), TestContext.Current.CancellationToken);
        Assert.NotNull(response);
        Assert.NotNull(response.Response);
        Assert.True(response.Response.Success);
        Assert.Empty(response.Response.Errors);
        Assert.False(string.IsNullOrEmpty(response.Id));
        Assert.Equal(id, response.Id);
    }
    [Fact]
    public async Task CanFindByUsername()
    {
        Assert.NotNull(_accountsClient);
        FindUserResponse response = await _accountsClient.FindByUserNameAsync(new StringInputParameter() { Value = "testusergrpc" }, null, DateTime.UtcNow.AddSeconds(10), TestContext.Current.CancellationToken); // UserManager is NOT case sensitive!
        Assert.NotNull(response);
        Assert.NotNull(response.Response);
        Assert.True(response.Response.Success);
        Assert.Empty(response.Response.Errors);
        Assert.False(string.IsNullOrEmpty(response.Id));
        Assert.Equal("CE73A87D-0AA6-4191-B65B-6B49F333E316", response.Id);
    }
    [Fact]
    public async Task CanFindByEmail()
    {
        Assert.NotNull(_accountsClient);
        FindUserResponse response = await _accountsClient.FindByEmailAsync(new StringInputParameter() { Value = "testusergrpc@email.com" }, null, null, TestContext.Current.CancellationToken); // UserManager is NOT case sensitive!
        Assert.NotNull(response);
        Assert.NotNull(response.Response);
        Assert.True(response.Response.Success);
        Assert.Empty(response.Response.Errors);
        Assert.False(string.IsNullOrEmpty(response.Id));
        Assert.Equal("CE73A87D-0AA6-4191-B65B-6B49F333E316", response.Id);
    }
    [Fact]
    public async Task CanChangePasswordWithValidAccountDetails()
    {
        Assert.NotNull(_accountsClient);
        Assert.NotNull(_authClient);
        // Create User
        RegisterUserResponse response = await _accountsClient.RegisterAsync(new RegisterUserRequest()
        {
            FirstName = "FirstName",
            LastName = "LastName",
            Email = "user1grpc@gmail.com",
            UserName = "user1grpc",
            Password = "P@$$w0rd"
        }, null, DateTime.UtcNow.AddSeconds(10), TestContext.Current.CancellationToken);
        Assert.NotNull(response);
        Assert.NotNull(response.Response);
        Assert.True(response.Response.Success);
        Assert.Empty(response.Response.Errors);
        Assert.False(string.IsNullOrEmpty(response.Id));

        // Login
        LogInResponse loginResponse = await _authClient.LogInAsync(new LogInRequest()
        {
            UserName = "user1grpc",
            Password = "P@$$w0rd"
        }, null, DateTime.UtcNow.AddSeconds(10), TestContext.Current.CancellationToken);
        Assert.NotNull(loginResponse);
        Assert.NotNull(loginResponse.Response);
        Assert.True(loginResponse.Response.Success);
        Assert.Empty(loginResponse.Response.Errors);
        Assert.NotNull(loginResponse.AccessToken);
        Assert.NotNull(loginResponse.AccessToken.Token);
        Assert.False(string.IsNullOrEmpty(loginResponse.RefreshToken));
        Assert.Equal(7200, loginResponse.AccessToken.ExpiresIn);

        // Change Password
        Identity.Response pwdResponse = await _accountsClient.ChangePasswordAsync(new ChangePasswordRequest()
        {
            Id = response.Id,
            Password = "P@$$w0rd",
            NewPassword = "P@$$w0rd1",
        }, null, DateTime.UtcNow.AddSeconds(10), TestContext.Current.CancellationToken);
        Assert.NotNull(pwdResponse);
        Assert.NotNull(pwdResponse);
        Assert.True(pwdResponse.Success);
        Assert.Empty(pwdResponse.Errors);

        // Should fail login with previous password
        LogInResponse loginResponse1 = await _authClient.LogInAsync(new LogInRequest()
        {
            UserName = "user1grpc",
            Password = "P@$$w0rd"
        }, null, DateTime.UtcNow.AddSeconds(10), TestContext.Current.CancellationToken);
        Assert.NotNull(loginResponse1);
        Assert.NotNull(loginResponse1.Response);
        Assert.False(loginResponse1.Response.Success);
        Assert.Single(loginResponse1.Response.Errors);
        Assert.Null(loginResponse1.AccessToken);
        Assert.True(string.IsNullOrEmpty(loginResponse1.RefreshToken));
        Assert.Equal(HttpStatusCode.Unauthorized.ToString(), loginResponse1.Response.Errors.First().Code);
        Assert.Equal("Invalid username or password!", loginResponse1.Response.Errors.First().Description);

        // Login
        LogInResponse loginResponse2 = await _authClient.LogInAsync(new LogInRequest()
        {
            UserName = "user1grpc",
            Password = "P@$$w0rd1"
        }, null, DateTime.UtcNow.AddSeconds(10), TestContext.Current.CancellationToken);
        Assert.NotNull(loginResponse2);
        Assert.NotNull(loginResponse2.Response);
        Assert.True(loginResponse2.Response.Success);
        Assert.Empty(loginResponse2.Response.Errors);
        Assert.NotNull(loginResponse2.AccessToken);
        Assert.NotNull(loginResponse2.AccessToken.Token);
        Assert.False(string.IsNullOrEmpty(loginResponse2.RefreshToken));
        Assert.Equal(7200, loginResponse2.AccessToken.ExpiresIn);
    }
    [Fact]
    public async Task CanResetPasswordWithValidAccountDetails()
    {
        Assert.NotNull(_accountsClient);
        Assert.NotNull(_authClient);
        // Create User
        RegisterUserResponse response = await _accountsClient.RegisterAsync(new RegisterUserRequest()
        {
            FirstName = "FirstName",
            LastName = "LastName",
            Email = "user2grpc@gmail.com",
            UserName = "user2grpc",
            Password = "P@$$w0rd"
        }, null, null, TestContext.Current.CancellationToken);//.ResponseAsync.DefaultTimeout();
        Assert.NotNull(response);
        Assert.NotNull(response.Response);
        Assert.True(response.Response.Success);
        Assert.Empty(response.Response.Errors);
        Assert.False(string.IsNullOrEmpty(response.Id));

        // Login
        LogInResponse loginResponse = await _authClient.LogInAsync(new LogInRequest()
        {
            UserName = "user2grpc",
            Password = "P@$$w0rd"
        }, null, null, TestContext.Current.CancellationToken);
        Assert.NotNull(loginResponse);
        Assert.NotNull(loginResponse.Response);
        Assert.True(loginResponse.Response.Success);
        Assert.Empty(loginResponse.Response.Errors);
        Assert.NotNull(loginResponse.AccessToken);
        Assert.NotNull(loginResponse.AccessToken.Token);
        Assert.False(string.IsNullOrEmpty(loginResponse.RefreshToken));
        Assert.Equal(7200, loginResponse.AccessToken.ExpiresIn);

        // Reset Password
        Identity.Response pwdResponse = await _accountsClient.ResetPasswordAsync(new ResetPasswordRequest()
        {
            Id = response.Id,
            NewPassword = "P@$$w0rd1",
        }, null, null, TestContext.Current.CancellationToken);
        Assert.NotNull(pwdResponse);
        Assert.NotNull(pwdResponse);
        Assert.True(pwdResponse.Success);
        Assert.Empty(pwdResponse.Errors);

        // Should fail login with previous password
        LogInResponse loginResponse1 = await _authClient.LogInAsync(new LogInRequest()
        {
            UserName = "user2grpc",
            Password = "P@$$w0rd"
        }, null, null, TestContext.Current.CancellationToken);
        Assert.NotNull(loginResponse1);
        Assert.NotNull(loginResponse1.Response);
        Assert.False(loginResponse1.Response.Success);
        Assert.Single(loginResponse1.Response.Errors);
        Assert.Null(loginResponse1.AccessToken);
        Assert.True(string.IsNullOrEmpty(loginResponse1.RefreshToken));
        Assert.Equal(HttpStatusCode.Unauthorized.ToString(), loginResponse1.Response.Errors.First().Code);
        Assert.Equal("Invalid username or password!", loginResponse1.Response.Errors.First().Description);

        // Login
        LogInResponse loginResponse2 = await _authClient.LogInAsync(new LogInRequest()
        {
            UserName = "user2grpc",
            Password = "P@$$w0rd1"
        }, null, null, TestContext.Current.CancellationToken);
        Assert.NotNull(loginResponse2);
        Assert.NotNull(loginResponse2.Response);
        Assert.True(loginResponse2.Response.Success);
        Assert.Empty(loginResponse2.Response.Errors);
        Assert.NotNull(loginResponse2.AccessToken);
        Assert.NotNull(loginResponse2.AccessToken.Token);
        Assert.False(string.IsNullOrEmpty(loginResponse2.RefreshToken));
        Assert.Equal(7200, loginResponse2.AccessToken.ExpiresIn);
    }
}