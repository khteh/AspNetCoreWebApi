using System.Collections.Generic;
using Web.Api.Core.Interfaces;

namespace Web.Api.Core.DTO.UseCaseResponses
{
    public class ExchangeRefreshTokenResponse : UseCaseResponseMessage
    {
        public AccessToken AccessToken { get; }
        public string RefreshToken { get; }

        public ExchangeRefreshTokenResponse(List<Error> errors) : base(null, false, null, errors) {}
        public ExchangeRefreshTokenResponse(AccessToken accessToken, string refreshToken, bool success = false, string message = null) : base(null, success, message)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}