using System.Collections.Generic;
using System.Text.Json.Serialization;
using Web.Api.Core.DTO;
namespace Web.Api.Models.Response;

public record RegisterUserResponse : ResponseBase //(string Id, bool Success, List<Error> Errors) : ResponseBase(Success, Errors);
{
    public string? Id { get; set; }
    public RegisterUserResponse(bool success, List<Error> errors) : base(success, errors) { }
    [JsonConstructor]
    public RegisterUserResponse(string? id, bool success, List<Error> errors) : base(success, errors) => Id = id;
}