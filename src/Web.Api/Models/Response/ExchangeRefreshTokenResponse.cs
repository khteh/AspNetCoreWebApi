using System.Collections.Generic;
using Newtonsoft.Json;
using Web.Api.Core.DTO;

namespace Web.Api.Models.Response
{
    public class ExchangeRefreshTokenResponse
    {
        [JsonProperty]
        public AccessToken AccessToken { get; private set; }
        [JsonProperty]
        public string RefreshToken { get; private set; }
        [JsonProperty]
        public List<Error> Errors {get; private set; }
        [JsonConstructor]
        public ExchangeRefreshTokenResponse(AccessToken accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
        public ExchangeRefreshTokenResponse(List<Error> errors) => Errors = errors;
    }
}