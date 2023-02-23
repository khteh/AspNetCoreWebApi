using System;
using System.Collections.Generic;

namespace Web.Api.Core.DTO.GatewayResponses.Repositories;

public class SignInResponse : BaseGatewayResponse
{
    public bool RequiresTwoFactor { get; }
    public bool IsLockedOut { get; }
    public Guid UserId { get; init; }
    public string? UserName { get; init; }
    public SignInResponse(Guid userId, string? username, bool success = false, bool requires2fa = false, bool lockedout = false, List<Error> errors = null) : base(success, errors)
    {
        UserId = userId;
        UserName = username;
        RequiresTwoFactor = requires2fa;
        IsLockedOut = lockedout;
    }
}