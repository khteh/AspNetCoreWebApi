using System;
using System.Collections.Generic;
using System.Text;

namespace Web.Api.Core.DTO.GatewayResponses.Repositories
{
    public sealed class FindUserResponse : BaseGatewayResponse
    {
        public string Id { get; }
        public FindUserResponse(string id, bool success = false, List<Error> errors = null) : base(success, errors)
        {
            Id = id;
        }
    }
}