using System.Collections.Generic;

namespace Web.Api.Core.DTO.GatewayResponses
{
  public abstract class BaseGatewayResponse
  {
    public bool Success { get; }
    public List<Error> Errors { get; }

    protected BaseGatewayResponse(bool success=false, List<Error> errors=null)
    {
      Success = success;
      Errors = errors;
    }
  }
}