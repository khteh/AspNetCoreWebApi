using System.Collections.Generic;

namespace Web.Api.Core.DTO.GatewayResponses.Repositories
{
    public class SignInResponse : BaseGatewayResponse
    {
        public string UserId { get; private set; }
        public SignInResponse(string userId, bool success = false, List<Error> errors = null) : base(success, errors)
        {
            UserId = userId;
        }
    }
}