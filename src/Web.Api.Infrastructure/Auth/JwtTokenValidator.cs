using System;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Web.Api.Core.Interfaces.Services;
using Web.Api.Infrastructure.Interfaces;
namespace Web.Api.Infrastructure.Auth;

public sealed class JwtTokenValidator : IJwtTokenValidator
{
    private readonly IJwtTokenHandler _jwtTokenHandler;
    private readonly JwtIssuerOptions _jwtOptions;
    public JwtTokenValidator(IJwtTokenHandler jwtTokenHandler, IOptions<JwtIssuerOptions> jwtOptions)
    {
        _jwtTokenHandler = jwtTokenHandler;
        _jwtOptions = jwtOptions.Value;
    }
    public ClaimsPrincipal? GetPrincipalFromToken(string token, string signingKey)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _jwtOptions.Issuer,

            ValidateAudience = true,
            ValidAudience = _jwtOptions.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),

            RequireExpirationTime = true,
            ValidateLifetime = false, // This function is called from ExchangeRefreshToken. If set to true, SecurityTokenExpiredException is thrown and there is no way to exchange RefreshToken for a new AccessToken!
            ClockSkew = TimeSpan.Zero
        };
        return _jwtTokenHandler.ValidateToken(token, tokenValidationParameters);
    }
}