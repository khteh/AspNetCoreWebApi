using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.SignalR;
namespace Web.Api;

/// <summary>
/// SignalR user identifier (Context.UserIdentifier, Clients.User(...)) = the JWT "sub" claim (username, issued by JwtFactory).
/// Reads the claim explicitly so it does not depend on inbound claim mapping (MapInboundClaims = false) or NameClaimType.
/// </summary>
public class NameUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection) => connection.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
}
