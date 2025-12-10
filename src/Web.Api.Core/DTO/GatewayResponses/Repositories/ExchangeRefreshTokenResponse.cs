using System.Collections.Generic;

namespace Web.Api.Core.DTO.GatewayResponses.Repositories;

public sealed class ExchangeRefreshTokenResponse : BaseGatewayResponse
{
    public AccessToken? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public ExchangeRefreshTokenResponse(AccessToken? accessToken, string? refreshToken, bool success = false, List<Error>? errors = null) : base(success, errors) => (AccessToken, RefreshToken) = (accessToken, refreshToken);
}