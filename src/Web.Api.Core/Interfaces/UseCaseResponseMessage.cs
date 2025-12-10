using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Web.Api.Core.DTO;
namespace Web.Api.Core.Interfaces;

public class UseCaseResponseMessage
{
    public Guid Id { get; init; } = Guid.Empty;
    public bool Success { get; init; } = false;
    public string? Message { get; init; }
    public List<Error>? Errors { get; init; }
    [JsonConstructor]
    public UseCaseResponseMessage(Guid id, bool success = false, string? message = null, List<Error>? errors = null)
    {
        Id = id;
        Success = success;
        Message = message;
        Errors = errors ?? new List<Error>();
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
        Errors!.Add(new Error(string.Empty, message));
    }
}