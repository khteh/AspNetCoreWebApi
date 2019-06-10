using System;
using System.Collections.Generic;

namespace Web.Api.Core.DTO.GatewayResponses.Repositories
{
    public sealed class PasswordResponse : BaseGatewayResponse
    {
        public string Id { get; }
        public PasswordResponse(string id, bool success = false, List<Error> errors = null) : base(success, errors) => Id = id;
    }
}