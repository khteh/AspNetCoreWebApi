using System.Collections.Generic;
using System.Text.Json.Serialization;
using Web.Api.Core.Interfaces;

namespace Web.Api.Core.DTO.UseCaseResponses
{
    public class LoginResponse : UseCaseResponseMessage
    {
        public AccessToken AccessToken { get; }
        public string RefreshToken { get; }
        public LoginResponse(List<Error> errors, string message = null) : base(null, false, message, errors) { }
        [JsonConstructor]
        public LoginResponse(AccessToken accessToken, string refreshToken, bool success = false, string message = null) : base(null, success, message)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}