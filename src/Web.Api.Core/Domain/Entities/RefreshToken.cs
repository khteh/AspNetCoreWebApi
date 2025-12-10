using System;
using Web.Api.Core.Shared;
namespace Web.Api.Core.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; init; }
    public DateTimeOffset Expires { get; init; }
    public int UserId { get; init; }
    public bool Active => DateTimeOffset.UtcNow <= Expires;
    public string? RemoteIpAddress { get; init; }
    public RefreshToken(string token, DateTimeOffset expires, int userId, string? remoteIpAddress)
    {
        Token = token;
        Expires = expires;
        UserId = userId;
        RemoteIpAddress = remoteIpAddress;
    }
}