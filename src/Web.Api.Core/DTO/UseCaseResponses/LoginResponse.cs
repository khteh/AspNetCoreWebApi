using System.Collections.Generic;
using Web.Api.Core.Interfaces;

namespace Web.Api.Core.DTO.UseCaseResponses
{
    public class LoginResponse : UseCaseResponseMessage
    {
        public AccessToken AccessToken { get; }
        public string RefreshToken { get; }
        public LoginResponse(List<Error> errors, string message = null) : base(null, false, message, errors) { }
        public LoginResponse(AccessToken accessToken, string refreshToken, bool success = false, string message = null) : base(null, success, message)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}