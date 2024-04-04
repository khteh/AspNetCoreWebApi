using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Web.Api.Core.DTO;
using Web.Api.Core.Interfaces.Services;
using Web.Api.Infrastructure.Interfaces;
namespace Web.Api.Infrastructure.Auth;
public sealed class JwtFactory : IJwtFactory
{
    private readonly ILogger<IJwtFactory> _logger;
    private readonly IJwtTokenHandler _jwtTokenHandler;
    private readonly JwtIssuerOptions _jwtOptions;
    public JwtFactory(ILogger<IJwtFactory> logger, IJwtTokenHandler jwtTokenHandler, IOptions<JwtIssuerOptions> jwtOptions)
    {
        _logger = logger;
        _jwtTokenHandler = jwtTokenHandler;
        _jwtOptions = jwtOptions.Value;
        ThrowIfInvalidOptions(_jwtOptions);
    }
    public async Task<AccessToken> GenerateEncodedToken(string id, string userName)
    {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userName))
        {
            _logger.LogError($@"{nameof(JwtFactory)}.{nameof(GenerateEncodedToken)} Invalid id ""{id}"" / username ""{userName}""!");
            return null;
        }
        var identity = GenerateClaimsIdentity(id, userName);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userName),
            new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
            new Claim(JwtRegisteredClaimNames.Iat, _jwtOptions.IssuedAt.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            identity.FindFirst(Helpers.Constants.Strings.JwtClaimIdentifiers.Rol),
            identity.FindFirst(Helpers.Constants.Strings.JwtClaimIdentifiers.Id)
        };
        // Create the JWT security token and encode it.
        var jwt = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Audience,
                claims,
                _jwtOptions.NotBefore.UtcDateTime,
                _jwtOptions.Expiration.UtcDateTime,
                _jwtOptions.SigningCredentials);
        return new AccessToken(_jwtTokenHandler.WriteToken(jwt), (int)_jwtOptions.ValidFor.TotalSeconds);
    }
    private static ClaimsIdentity GenerateClaimsIdentity(string id, string userName) =>
        !string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(userName) ? new ClaimsIdentity(new GenericIdentity(userName, "Token"), new[]
            {
                new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Id, id),
                new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Rol, Helpers.Constants.Strings.JwtClaims.ApiAccess)
            }) : null;
    /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
    private static long ToUnixEpochDate(DateTimeOffset date) => date.ToUnixTimeSeconds();
    private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));
        if (options.ValidFor <= TimeSpan.Zero)
            throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
        if (options.SigningCredentials == null)
            throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
        if (options.JtiGenerator == null)
            throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
    }
}