using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web.Api.Identity.Accounts;
using Web.Api.Identity.Auth;
using Xunit;
using static Web.Api.Identity.Accounts.Accounts;
using static Web.Api.Identity.Auth.Auth;

namespace Web.Api.IntegrationTests.Services
{
    public class AccountsServiceIntegrationTests : FunctionalTestBase//IClassFixture<CustomGrpcServerFactory<Startup>>
    {
        #if false
        private ServiceProvider _serviceProvider;
        public AccountsServiceIntegrationTests(CustomGrpcServerFactory<Startup> factory)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;
            factory.CreateClient();
            _serviceProvider = factory.ServiceProvider;
        }
        #endif
        [Fact]
        public async Task CanRegisterUserWithValidAccountDetails()
        {
            AccountsClient client = new AccountsClient(Channel);//_serviceProvider.GetRequiredService<AccountsClient>();
            Assert.NotNull(client);
            // Act
            RegisterUserResponse response = await client.RegisterAsync(new RegisterUserRequest() {
                FirstName = "John",
                LastName = "Doe",
                Email = "jdoe@gmail.com",
                UserName = "johndoe",
                Password = "P@$$w0rd"
            });//.ResponseAsync.DefaultTimeout();

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
            AccountsClient client = new AccountsClient(Channel);//_serviceProvider.GetRequiredService<AccountsClient>();
            Assert.NotNull(client);
            DeleteUserResponse response = await client.DeleteAsync(new StringInputParameter() { Value = "deleteme"});
            Assert.NotNull(response);
            Assert.NotNull(response.Response);
            Assert.True(response.Response.Success);
            Assert.Empty(response.Response.Errors);
            Assert.False(string.IsNullOrEmpty(response.Id));
        }

        [Fact]
        public async Task CantRegisterUserWithInvalidAccountDetails()
        {
            AccountsClient client = new AccountsClient(Channel);//_serviceProvider.GetRequiredService<AccountsClient>();
            Assert.NotNull(client);
            // Act
            RegisterUserResponse response = await client.RegisterAsync(new RegisterUserRequest() {
                FirstName = "John",
                LastName = "Doe",
                Email = string.Empty,
                UserName = string.Empty,
                Password = "P@$$w0rd"
            });//.ResponseAsync.DefaultTimeout();

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
            AccountsClient client = new AccountsClient(Channel);//_serviceProvider.GetRequiredService<AccountsClient>();
            Assert.NotNull(client);
            DeleteUserResponse response = await client.DeleteAsync(new StringInputParameter() { Value = "DeleteMeNot"});
            Assert.NotNull(response);
            Assert.NotNull(response.Response);
            Assert.False(response.Response.Success);
            Assert.Single(response.Response.Errors);
            Assert.True(string.IsNullOrEmpty(response.Id));
            Assert.Equal(HttpStatusCode.BadRequest.ToString(), response.Response.Errors.First().Code);
            Assert.Equal("Invalid user!", response.Response.Errors.First().Description);
        }
        [Theory]
        [InlineData("41532945-599e-4910-9599-0e7402017fbe")]
        [InlineData("7B697F98-AE31-41E7-BE13-20C63314ABF9")]
        public async Task CanFindById(string id)
        {
            AccountsClient client = new AccountsClient(Channel);//_serviceProvider.GetRequiredService<AccountsClient>();
            Assert.NotNull(client);
            FindUserResponse response = await client.FindByIdAsync(new StringInputParameter() { Value = id});
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
            AccountsClient client = new AccountsClient(Channel);//_serviceProvider.GetRequiredService<AccountsClient>();
            Assert.NotNull(client);
            FindUserResponse response = await client.FindByUserNameAsync(new StringInputParameter() { Value = "mickeymouse"}); // UserManager is NOT case sensitive!
            Assert.NotNull(response);
            Assert.NotNull(response.Response);
            Assert.True(response.Response.Success);
            Assert.Empty(response.Response.Errors);
            Assert.False(string.IsNullOrEmpty(response.Id));
            Assert.Equal("41532945-599e-4910-9599-0e7402017fbe", response.Id);
        }
        [Fact]
        public async Task CanFindByEmail()
        {
            AccountsClient client = new AccountsClient(Channel);//_serviceProvider.GetRequiredService<AccountsClient>();
            Assert.NotNull(client);
            FindUserResponse response = await client.FindByEmailAsync(new StringInputParameter() { Value = "mickey@mouse.com"}); // UserManager is NOT case sensitive!
            Assert.NotNull(response);
            Assert.NotNull(response.Response);
            Assert.True(response.Response.Success);
            Assert.Empty(response.Response.Errors);
            Assert.False(string.IsNullOrEmpty(response.Id));
            Assert.Equal("41532945-599e-4910-9599-0e7402017fbe", response.Id);
        }
        [Fact]
        public async Task CanChangePasswordWithValidAccountDetails()
        {
            AccountsClient accountsClient = new AccountsClient(Channel);//_serviceProvider.GetRequiredService<AccountsClient>();
            AuthClient authClient = new AuthClient(Channel);//_serviceProvider.GetRequiredService<AuthClient>();
            Assert.NotNull(accountsClient);
            Assert.NotNull(authClient);
            // Create User
            RegisterUserResponse response = await accountsClient.RegisterAsync(new RegisterUserRequest() {
                FirstName = "FirstName",
                LastName = "LastName",
                Email = "user@gmail.com",
                UserName = "user1",
                Password = "P@$$w0rd"
            });//.ResponseAsync.DefaultTimeout();
            Assert.NotNull(response);
            Assert.NotNull(response.Response);
            Assert.True(response.Response.Success);
            Assert.Empty(response.Response.Errors);
            Assert.False(string.IsNullOrEmpty(response.Id));

            // Login
            LoginResponse loginResponse = await authClient.LoginAsync(new LoginRequest() {
                UserName = "user1",
                Password = "P@$$w0rd"
            });
            Assert.NotNull(loginResponse);
            Assert.NotNull(loginResponse.Response);
            Assert.True(loginResponse.Response.Success);
            Assert.Empty(loginResponse.Response.Errors);
            Assert.NotNull(loginResponse.AccessToken);
            Assert.NotNull(loginResponse.AccessToken.Token);
            Assert.False(string.IsNullOrEmpty(loginResponse.RefreshToken));
            Assert.Equal(7200, loginResponse.AccessToken.ExpiresIn);

            // Change Password
            Web.Api.Identity.Response pwdResponse = await accountsClient.ChangePasswordAsync(new ChangePasswordRequest() {
                Id = response.Id,
                Password = "P@$$w0rd",
                NewPassword = "P@$$w0rd1",
            });
            Assert.NotNull(pwdResponse);
            Assert.NotNull(pwdResponse);
            Assert.True(pwdResponse.Success);
            Assert.Empty(pwdResponse.Errors);

            // Should fail login with previous password
            LoginResponse loginResponse1 = await authClient.LoginAsync(new LoginRequest() {
                UserName = "user1",
                Password = "P@$$w0rd"
            });
            Assert.NotNull(loginResponse1);
            Assert.NotNull(loginResponse1.Response);
            Assert.False(loginResponse1.Response.Success);
            Assert.Single(loginResponse1.Response.Errors);
            Assert.Null(loginResponse1.AccessToken);
            Assert.True(string.IsNullOrEmpty(loginResponse1.RefreshToken));
            Assert.Equal(HttpStatusCode.Unauthorized.ToString(), loginResponse1.Response.Errors.First().Code);
            Assert.Equal("Invalid username or password!", loginResponse1.Response.Errors.First().Description);

            // Login
            LoginResponse loginResponse2 = await authClient.LoginAsync(new LoginRequest() {
                UserName = "user1",
                Password = "P@$$w0rd1"
            });
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
            AccountsClient accountsClient = new AccountsClient(Channel);//_serviceProvider.GetRequiredService<AccountsClient>();
            AuthClient authClient = new AuthClient(Channel);//_serviceProvider.GetRequiredService<AuthClient>();
            Assert.NotNull(accountsClient);
            Assert.NotNull(authClient);
            // Create User
            RegisterUserResponse response = await accountsClient.RegisterAsync(new RegisterUserRequest() {
                FirstName = "FirstName",
                LastName = "LastName",
                Email = "user1@gmail.com",
                UserName = "user2",
                Password = "P@$$w0rd"
            });//.ResponseAsync.DefaultTimeout();
            Assert.NotNull(response);
            Assert.NotNull(response.Response);
            Assert.True(response.Response.Success);
            Assert.Empty(response.Response.Errors);
            Assert.False(string.IsNullOrEmpty(response.Id));

            // Login
            LoginResponse loginResponse = await authClient.LoginAsync(new LoginRequest() {
                UserName = "user2",
                Password = "P@$$w0rd"
            });
            Assert.NotNull(loginResponse);
            Assert.NotNull(loginResponse.Response);
            Assert.True(loginResponse.Response.Success);
            Assert.Empty(loginResponse.Response.Errors);
            Assert.NotNull(loginResponse.AccessToken);
            Assert.NotNull(loginResponse.AccessToken.Token);
            Assert.False(string.IsNullOrEmpty(loginResponse.RefreshToken));
            Assert.Equal(7200, loginResponse.AccessToken.ExpiresIn);

            // Reset Password
            Web.Api.Identity.Response pwdResponse = await accountsClient.ResetPasswordAsync(new ResetPasswordRequest() {
                Id = response.Id,
                NewPassword = "P@$$w0rd1",
            });
            Assert.NotNull(pwdResponse);
            Assert.NotNull(pwdResponse);
            Assert.True(pwdResponse.Success);
            Assert.Empty(pwdResponse.Errors);

            // Should fail login with previous password
            LoginResponse loginResponse1 = await authClient.LoginAsync(new LoginRequest() {
                UserName = "user2",
                Password = "P@$$w0rd"
            });
            Assert.NotNull(loginResponse1);
            Assert.NotNull(loginResponse1.Response);
            Assert.False(loginResponse1.Response.Success);
            Assert.Single(loginResponse1.Response.Errors);
            Assert.Null(loginResponse1.AccessToken);
            Assert.True(string.IsNullOrEmpty(loginResponse1.RefreshToken));
            Assert.Equal(HttpStatusCode.Unauthorized.ToString(), loginResponse1.Response.Errors.First().Code);
            Assert.Equal("Invalid username or password!", loginResponse1.Response.Errors.First().Description);

            // Login
            LoginResponse loginResponse2 = await authClient.LoginAsync(new LoginRequest() {
                UserName = "user2",
                Password = "P@$$w0rd1"
            });
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
}