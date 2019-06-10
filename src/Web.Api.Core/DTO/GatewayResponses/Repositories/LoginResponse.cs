using System;
using System.Collections.Generic;
using Web.Api.Core.Domain.Entities;

namespace Web.Api.Core.DTO.GatewayResponses.Repositories
{
    public sealed class LogInResponse : BaseGatewayResponse
    {
        public User User { get; }
        public LogInResponse(User user, bool success = false, List<Error> errors = null) : base(success, errors)
        {
            User = user;
        }
    }
}