using System.Collections.Generic;
using System.Text.Json.Serialization;
using Web.Api.Core.DTO;
namespace Web.Api.Models.Response;

public record ResponseBase
{
    public bool Success { get; init; } = false;
    /// <summary>Never null: an empty list means no errors, so clients can always iterate it.</summary>
    public List<Error> Errors { get; init; } = [];
    [JsonConstructor]
    public ResponseBase(bool success, List<Error>? errors) => (Success, Errors) = (success, errors ?? []);
}