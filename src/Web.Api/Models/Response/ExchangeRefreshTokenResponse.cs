using System.Collections.Generic;
using System.Text.Json.Serialization;
using Web.Api.Core.DTO;
namespace Web.Api.Models.Response;
public record ExchangeRefreshTokenResponse : ResponseBase
{
    public AccessToken AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public ExchangeRefreshTokenResponse(bool success, List<Core.DTO.Error> errors) : base(success, errors) { }
    [JsonConstructor]
    public ExchangeRefreshTokenResponse(AccessToken accessToken, string refreshToken, bool success, List<Error> errors) : base(success, errors) => (AccessToken, RefreshToken) = (accessToken, refreshToken);
}