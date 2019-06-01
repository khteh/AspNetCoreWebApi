using System;
using System.Collections.Generic;
using System.Text;

namespace Web.Api.Core.Dto.GatewayResponses.Repositories
{
    public sealed class DeleteUserResponse : BaseGatewayResponse
    {
        public string Id { get; }
        public DeleteUserResponse(string id, bool success = false, List<Error> errors = null) : base(success, errors)
        {
            Id = id;
        }
    }
}