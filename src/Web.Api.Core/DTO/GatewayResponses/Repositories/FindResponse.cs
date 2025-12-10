using System;
using System.Collections.Generic;
using Web.Api.Core.Shared;

namespace Web.Api.Core.DTO.GatewayResponses.Repositories;

public sealed class FindResponse<T> : BaseGatewayResponse where T : BaseEntity
{
    public Guid Id { get; }
    public List<T> Result { get; }
    public FindResponse(Guid id, List<T> result, bool success = false, List<Error>? errors = null) : base(success, errors)
    {
        Id = id;
        Result = result;
    }
}