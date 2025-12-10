using System;
using System.Collections.Generic;
using Web.Api.Core.Domain.Entities;
namespace Web.Api.Core.DTO.GatewayResponses.Repositories;

public sealed class FindUserResponse : BaseGatewayResponse
{
    public Guid Id { get; }
    public User? User { get; }
    public FindUserResponse(Guid id, User? user, bool success = false, List<Error>? errors = null) : base(success, errors)
    {
        Id = id;
        User = user;
    }
}