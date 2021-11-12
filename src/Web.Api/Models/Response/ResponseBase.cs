using System.Collections.Generic;
using System.Text.Json.Serialization;
using Web.Api.Core.DTO;
namespace Web.Api.Models.Response;
public record ResponseBase
{
    public bool Success { get; init; } = false;
    public List<Error> Errors { get; init; } = new List<Error>();
    [JsonConstructor]
    public ResponseBase(bool success, List<Error> errors) => (Success, Errors) = (success, errors);
}