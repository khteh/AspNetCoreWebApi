

using System.Collections.Generic;
using Newtonsoft.Json;
using Web.Api.Core.DTO;

namespace Web.Api.Models.Response
{
    public class LoginResponse : ResponseBase
    {
        [JsonProperty]
        public AccessToken AccessToken { get; private set; }
        [JsonProperty]
        public string RefreshToken { get; private set; }
        public LoginResponse(AccessToken accessToken, string refreshToken, bool success, List<Error> errors) : base(success, errors)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}
