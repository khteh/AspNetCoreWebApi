using System.Collections.Generic;
using Web.Api.Core.Interfaces;

namespace Web.Api.Core.Dto.UseCaseResponses
{
    public class LoginResponse : UseCaseResponseMessage
    {
        public AccessToken AccessToken { get; }
        public string RefreshToken { get; }
        public LoginResponse(List<Error> errors, bool success = false, string message = null) : base(success, message, errors)
        {
        }
        public LoginResponse(AccessToken accessToken, string refreshToken, bool success = false, string message = null) : base(success, message)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}