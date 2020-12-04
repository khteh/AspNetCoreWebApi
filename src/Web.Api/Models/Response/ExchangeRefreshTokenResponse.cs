using System.Collections.Generic;
using System.Text.Json.Serialization;
using Web.Api.Core.DTO;

namespace Web.Api.Models.Response
{
    public class ExchangeRefreshTokenResponse : ResponseBase
    {
        public AccessToken AccessToken { get; init; }
        public string RefreshToken { get; init; }
        [JsonConstructor]
        public ExchangeRefreshTokenResponse(AccessToken accessToken, string refreshToken, bool success, List<Error> errors) : base(success, errors)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}