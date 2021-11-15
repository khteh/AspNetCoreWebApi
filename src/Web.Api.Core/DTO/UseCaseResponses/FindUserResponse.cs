using System.Collections.Generic;
using System.Text.Json.Serialization;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.Interfaces;
namespace Web.Api.Core.DTO.UseCaseResponses;
public class FindUserResponse : UseCaseResponseMessage
{
    public User User { get; init;}
    [JsonConstructor]
    public FindUserResponse(User user, string id, bool success = false, string message = null, List<Error> errors = null) : base(id, success, message, errors) { User = user; }
}