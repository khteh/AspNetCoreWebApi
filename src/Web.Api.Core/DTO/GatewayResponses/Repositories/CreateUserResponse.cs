using System;
using System.Collections.Generic;
namespace Web.Api.Core.DTO.GatewayResponses.Repositories;

public sealed class CreateUserResponse : BaseGatewayResponse
{
    public Guid Id { get; }
    public CreateUserResponse(Guid id, bool success = false, List<Error>? errors = null) : base(success, errors) => Id = id;
}