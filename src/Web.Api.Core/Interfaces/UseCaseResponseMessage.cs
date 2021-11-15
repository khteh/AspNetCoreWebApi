using System.Collections.Generic;
using System.Text.Json.Serialization;
using Web.Api.Core.DTO;
namespace Web.Api.Core.Interfaces;
public class UseCaseResponseMessage
{
    public string Id { get; init; } = string.Empty;
    public bool Success { get; init; } = false;
    public string Message { get; init; } = string.Empty;
    public List<Error> Errors { get; init; } = new List<Error>();
    [JsonConstructor]
    public UseCaseResponseMessage(string id, bool success = false, string message = null, List<Error> errors = null)
    {
        Id = id;
        Success = success;
        Message = message;
        Errors = errors;
    }
    public UseCaseResponseMessage(string message, List<Error> errors)
    {
        Message = message;
        Errors = errors;
    }
    public UseCaseResponseMessage(List<Error> errors) => Errors = errors;
    public UseCaseResponseMessage(string message)
    {
        Message = message;
        Errors.Add(new Error(string.Empty, message));
    }
}