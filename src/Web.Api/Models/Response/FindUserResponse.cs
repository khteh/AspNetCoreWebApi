using System.Collections.Generic;
using System.Text.Json.Serialization;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.DTO;
namespace Web.Api.Models.Response;
public record FindUserResponse : ResponseBase
{
    public string Id { get; set; }
    public string Message { get; set; }
    public User User { get; set; }
    public FindUserResponse(bool success, List<Core.DTO.Error> errors) : base(success, errors) { }
    [JsonConstructor]
    public FindUserResponse(string id, string message, User user, bool success, List<Error> errors) : base(success, errors) => (Id, Message, User) = (id, message, user);
}