using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Web.Api.Core.Interfaces;
namespace Web.Api.Core.DTO.UseCaseResponses;

public class LogInResponse : UseCaseResponseMessage
{
    public AccessToken? AccessToken { get; }
    public string? RefreshToken { get; }
    public LogInResponse(List<Error> errors, string? message = null) : base(Guid.Empty, false, message, errors) { }
    [JsonConstructor]
    public LogInResponse(AccessToken? accessToken, string refreshToken, bool success = false, string? message = null) : base(Guid.Empty, success, message)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
}