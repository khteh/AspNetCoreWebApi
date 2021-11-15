using System.Collections.Generic;
using Web.Api.Core.DTO.GatewayResponses;
namespace Web.Api.Core.DTO.UseCaseResponses;
public sealed class LockUserResponse : BaseGatewayResponse
{
    public string Id { get; }
    public LockUserResponse(string id, bool success = false, List<Error> errors = null) : base(success, errors) => Id = id;
}