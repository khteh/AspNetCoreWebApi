using System.Collections.Generic;

namespace Web.Api.Core.DTO.GatewayResponses.Repositories
{
  public sealed class CreateUserResponse : BaseGatewayResponse
  {
    public string Id { get; }
    public CreateUserResponse(string id, bool success = false, List<Error> errors = null) : base(success, errors)
    {
        Id = id;
    }
  }
}