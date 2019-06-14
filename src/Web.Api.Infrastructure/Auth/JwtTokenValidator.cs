using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Web.Api.Core.Interfaces.Services;
using Web.Api.Infrastructure.Interfaces;

namespace Web.Api.Infrastructure.Auth
{
    public sealed class JwtTokenValidator : IJwtTokenValidator
    {
        private readonly IJwtTokenHandler _jwtTokenHandler;
        public JwtTokenValidator(IJwtTokenHandler jwtTokenHandler) => _jwtTokenHandler = jwtTokenHandler;

        public ClaimsPrincipal GetPrincipalFromToken(string token, string signingKey)
        {
            return _jwtTokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                ValidateLifetime = false // we check expired tokens here
            });
        }
    }
}