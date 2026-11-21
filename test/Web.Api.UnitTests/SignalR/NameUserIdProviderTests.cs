using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
namespace Web.Api.UnitTests.SignalR;

public class NameUserIdProviderTests
{
    private static HubConnectionContext Connection(ClaimsPrincipal user) =>
        new HubConnectionContext(new DefaultConnectionContext { User = user }, new HubConnectionContextOptions(), NullLoggerFactory.Instance);

    [Fact]
    public void GetUserId_ReturnsSubClaim()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "testuser"),
            new Claim("rol", "api_access"),
            new Claim("id", "41532945-599e-4910-9599-0e7402017fbe")
        }, "Bearer"));

        Assert.Equal("testuser", new NameUserIdProvider().GetUserId(Connection(user)));
    }
    [Fact]
    public void GetUserId_IgnoresLegacyMappedNameIdentifier()
    {
        // With MapInboundClaims = false the JWT "sub" claim is never renamed to ClaimTypes.NameIdentifier.
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "testuser") }, "Bearer"));
        Assert.Null(new NameUserIdProvider().GetUserId(Connection(user)));
    }
    [Fact]
    public void GetUserId_UnauthenticatedConnection_ReturnsNull() =>
        Assert.Null(new NameUserIdProvider().GetUserId(Connection(null)));
}
