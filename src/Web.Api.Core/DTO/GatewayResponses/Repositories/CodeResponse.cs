using System;
using System.Collections.Generic;
namespace Web.Api.Core.DTO.GatewayResponses.Repositories;

public sealed class CodeResponse : BaseGatewayResponse
{
    public string? Code { get; }
    public Guid Id { get; }
    public CodeResponse(Guid id, string? code, bool success = false, List<Error>? errors = null) : base(success, errors) => (Id, Code) = (id, code);
}