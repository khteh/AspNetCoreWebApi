using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Web.Api.Core.Interfaces;
namespace Web.Api.Core.DTO.UseCaseResponses;

public class ExchangeRefreshTokenResponse : UseCaseResponseMessage
{
    public AccessToken? AccessToken { get; }
    public string? RefreshToken { get; }
    public ExchangeRefreshTokenResponse(List<Error>? errors) : base(Guid.Empty, false, null, errors) { }
    [JsonConstructor]
    public ExchangeRefreshTokenResponse(AccessToken accessToken, string refreshToken, bool success = false, string? message = null) : base(Guid.Empty, success, message)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
}