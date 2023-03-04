using System;
using System.Collections.Generic;
namespace Web.Api.Core.DTO.GatewayResponses.Repositories;
public sealed class CodeResponse : BaseGatewayResponse
{
    public string Code { get; }
    public string Id { get; }
    public CodeResponse(string id, string code, bool success = false, List<Core.DTO.Error> errors = null) : base(success, errors) => (Id, Code) = (id, code);
}