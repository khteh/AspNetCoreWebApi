using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web.Api.Identity.Auth;
using Xunit;
using static Web.Api.Identity.Auth.Auth;
namespace Web.Api.IntegrationTests.Services;
public class AuthServiceIntegrationTests : GrpcTestFixture<Program>//FunctionalTestBase//IClassFixture<CustomGrpcServerFactory<Program>>
{
#if false
    private ServiceProvider _serviceProvider;
    public AuthServiceIntegrationTests(CustomGrpcServerFactory<Program> factory)
    {
        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;
        factory.CreateClient();
        _serviceProvider = factory.ServiceProvider;
        Assert.NotNull(_serviceProvider);
    }
#endif
    [Fact]
    public async Task CanLoginWithValidCredentials()
    {
        AuthClient client = new AuthClient(GrpcChannel);//_serviceProvider.GetRequiredService<AuthClient>();
        Assert.NotNull(client);
        LogInResponse response = await client.LogInAsync(new LogInRequest()
        {
            UserName = "mickeymouse",
            Password = "P@$$w0rd"
        });
        Assert.NotNull(response);
        Assert.NotNull(response.Response);
        Assert.True(response.Response.Success);
        Assert.Empty(response.Response.Errors);
        Assert.NotNull(response.AccessToken);
        Assert.NotNull(response.AccessToken.Token);
        Assert.False(string.IsNullOrEmpty(response.RefreshToken));
        Assert.Equal(7200, response.AccessToken.ExpiresIn);
    }
    [Fact]
    public async Task CantLoginWithInvalidCredentials()
    {
        AuthClient client = new AuthClient(GrpcChannel);//_serviceProvider.GetRequiredService<AuthClient>();
        Assert.NotNull(client);
        LogInResponse response = await client.LogInAsync(new LogInRequest()
        {
            UserName = "unknown",
            Password = "ShouldFail"
        });
        Assert.NotNull(response);
        Assert.NotNull(response.Response);
        Assert.False(response.Response.Success);
        Assert.Single(response.Response.Errors);
        Assert.Null(response.AccessToken);
        Assert.True(string.IsNullOrEmpty(response.RefreshToken));
        Assert.Equal(HttpStatusCode.Unauthorized.ToString(), response.Response.Errors.First().Code);
        Assert.Equal("Invalid username or password!", response.Response.Errors.First().Description);
    }
    [Fact]
    public async Task CanExchangeValidRefreshToken()
    {
        AuthClient client = new AuthClient(GrpcChannel);//_serviceProvider.GetRequiredService<AuthClient>();
        Assert.NotNull(client);
        LogInResponse response = await client.LogInAsync(new LogInRequest()
        {
            UserName = "mickeymouse",
            Password = "P@$$w0rd"
        });
        Assert.NotNull(response);
        Assert.NotNull(response.Response);
        Assert.True(response.Response.Success);
        Assert.Empty(response.Response.Errors);
        Assert.NotNull(response.AccessToken);
        Assert.NotNull(response.AccessToken.Token);
        Assert.False(string.IsNullOrEmpty(response.RefreshToken));
        Assert.Equal(7200, response.AccessToken.ExpiresIn);

        ExchangeRefreshTokenResponse response1 = await client.RefreshTokenAsync(new ExchangeRefreshTokenRequest()
        {
            AccessToken = response.AccessToken.Token,
            RefreshToken = response.RefreshToken
        });
        Assert.NotNull(response1);
        Assert.NotNull(response1.Response);
        Assert.True(response1.Response.Success);
        Assert.Empty(response1.Response.Errors);
        Assert.NotNull(response1.AccessToken);
        Assert.NotNull(response1.AccessToken.Token);
        Assert.False(string.IsNullOrEmpty(response1.RefreshToken));
        Assert.Equal(7200, response1.AccessToken.ExpiresIn);
    }
    [Fact]
    public async Task CantExchangeInvalidRefreshToken()
    {
        AuthClient client = new AuthClient(GrpcChannel);//_serviceProvider.GetRequiredService<AuthClient>();
        Assert.NotNull(client);
        ExchangeRefreshTokenResponse response = await client.RefreshTokenAsync(new ExchangeRefreshTokenRequest()
        {
            AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJtbWFjbmVpbCIsImp0aSI6IjA0YjA0N2E4LTViMjMtNDgwNi04M2IyLTg3ODVhYmViM2ZjNyIsImlhdCI6MTUzOTUzNzA4Mywicm9sIjoiYXBpX2FjY2VzcyIsImlkIjoiNDE1MzI5NDUtNTk5ZS00OTEwLTk1OTktMGU3NDAyMDE3ZmJlIiwibmJmIjoxNTM5NTM3MDgyLCJleHAiOjE1Mzk1NDQyODIsImlzcyI6IndlYkFwaSIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMC8ifQ.xzDQOKzPZarve68Np8Iu8sh2oqoCpHSmp8fMdYRHC_k",
            RefreshToken = "ShouldFail"
        });
        Assert.NotNull(response);
        Assert.NotNull(response.Response);
        Assert.False(response.Response.Success);
        Assert.Single(response.Response.Errors);
        Assert.Null(response.AccessToken);
        Assert.True(string.IsNullOrEmpty(response.RefreshToken));
        Assert.Equal(HttpStatusCode.BadRequest.ToString(), response.Response.Errors.First().Code);
        Assert.Equal("Invalid token!", response.Response.Errors.First().Description);
    }
}