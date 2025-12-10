using Web.Api.Core.Interfaces;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;

namespace Web.Api.Core.DTO.UseCaseResponses;

public class SignInResponse : UseCaseResponseMessage
{
    public bool RequiresTwoFactor { get; }
    public bool IsLockedOut { get; }
    public string? UserName { get; }
    public SignInResponse(List<Error> errors, string? message = null) : base(Guid.Empty, false, message, errors) { }
    public SignInResponse(bool requires2fa, bool lockedout, List<Error>? errors, string? message = null) : base(Guid.Empty, false, message, errors)
    {
        RequiresTwoFactor = requires2fa;
        IsLockedOut = lockedout;
    }
    [JsonConstructor]
    public SignInResponse(Guid id, string username, bool success = false, string? message = null) : base(Guid.Empty, success, message)
    {
        Id = id;
        UserName = username;
    }
}