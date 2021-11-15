using System.Collections.Generic;
using Web.Api.Core.Domain.Entities;
namespace Web.Api.Core.DTO.GatewayResponses.Repositories;
public sealed class FindUserResponse : BaseGatewayResponse
{
    public string Id { get; }
    public User User { get; }
    public FindUserResponse(string id, User user, bool success = false, List<Error> errors = null) : base(success, errors)
    {
        Id = id;
        User = user;
    }
}