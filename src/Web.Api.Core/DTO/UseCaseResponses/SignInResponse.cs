using Web.Api.Core.Interfaces;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;

namespace Web.Api.Core.DTO.UseCaseResponses;

public class SignInResponse : UseCaseResponseMessage
{
    public bool RequiresTwoFactor { get; }
    public bool IsLockedOut { get; }
    public Guid Id { get; }
    public string UserName { get; }
    public SignInResponse(bool requires2fa, bool lockedout, List<Error> errors, string message = null) : base(string.Empty, false, message, errors)
    {
        RequiresTwoFactor = requires2fa;
        IsLockedOut = lockedout;
    }
    [JsonConstructor]
    public SignInResponse(Guid id, string username, bool success = false, string message = null) : base(string.Empty, success, message)
    {
        Id = id;
        UserName = username;
    }
}