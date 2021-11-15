﻿using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Web.Api.Infrastructure.Extensions;
using Web.Api.Infrastructure.Interfaces;
namespace Web.Api.Infrastructure.Auth;
public sealed class JwtTokenHandler : IJwtTokenHandler
{
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
    private readonly ILogger<JwtTokenHandler> _logger;
    public JwtTokenHandler(ILogger<JwtTokenHandler> logger)
    {
        _jwtSecurityTokenHandler ??= new JwtSecurityTokenHandler();
        _logger = logger;
    }
    public string WriteToken(JwtSecurityToken jwt) => _jwtSecurityTokenHandler.WriteToken(jwt);
    public ClaimsPrincipal ValidateToken(string token, TokenValidationParameters tokenValidationParameters)
    {
        try {
            var principal = _jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        } catch (Exception e) {
            _logger.LogError($"Token validation failed: {e.Message} {e.GetInnerMessage()} {e.StackTrace}");
            return null;
        }
    }
}