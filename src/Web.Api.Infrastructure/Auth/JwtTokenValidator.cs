using System;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Web.Api.Core.Interfaces.Services;
using Web.Api.Infrastructure.Interfaces;

namespace Web.Api.Infrastructure.Auth
{
    public sealed class JwtTokenValidator : IJwtTokenValidator
    {
        private readonly IJwtTokenHandler _jwtTokenHandler;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly AuthSettings _authSettings;
        public JwtTokenValidator(IJwtTokenHandler jwtTokenHandler, IOptions<JwtIssuerOptions> jwtOptions, IOptions<AuthSettings> authSettings)
        {
             _jwtTokenHandler = jwtTokenHandler;
             _jwtOptions = jwtOptions.Value;
             _authSettings = authSettings.Value;
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token, string signingKey)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.Issuer,

                ValidateAudience = true,
                ValidAudience = _jwtOptions.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSettings.SecretKey)),

                RequireExpirationTime = true,//false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            return _jwtTokenHandler.ValidateToken(token, tokenValidationParameters);
        }
    }
}