using System;
using System.Collections.Generic;
namespace Web.Api.Core.DTO.GatewayResponses.Repositories;

public sealed class GenerateNew2FARecoveryCodesResponse : BaseGatewayResponse
{
    public Guid Id { get; }
    public List<string>? Codes { get; }
    public GenerateNew2FARecoveryCodesResponse(Guid id, List<string>? codes, bool success = false, List<Error>? errors = null) : base(success, errors) => (Id, Codes) = (id, codes);
}