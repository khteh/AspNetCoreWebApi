using System;
using System.Collections.Generic;

namespace Web.Api.Core.Dto.GatewayResponses.Repositories
{
    public sealed class PasswordResponse : BaseGatewayResponse
    {
        public string Id { get; }
        public PasswordResponse(string id, bool success = false, IEnumerable<Error> errors = null) : base(success, errors) => Id = id;
    }
}