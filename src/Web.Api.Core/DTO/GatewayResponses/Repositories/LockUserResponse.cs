using System;
using System.Collections.Generic;

namespace Web.Api.Core.DTO.GatewayResponses.Repositories
{
    public sealed class LockUserResponse : BaseGatewayResponse
    {
        public string Id { get; }
        public LockUserResponse(string id, bool success = false, List<Error> errors = null) : base(success, errors) => Id = id;
    }
}