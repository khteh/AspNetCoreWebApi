using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using Web.Api.Core.Helpers;
using Web.Api.Core.Shared;
namespace Web.Api.Core.Domain.Entities;

[Serializable]
public class User : BaseEntity
{
    public required string IdentityId { get; set; }
    [NotMapped]
    public required string FirstName { get; set; } // EF migrations require at least initter - won't work on auto-property
    [NotMapped]
    public required string LastName { get; set; }
    [NotMapped]
    public required string UserName { get; set; } // Required by automapper
    [NotMapped]
    public required string Email { get; set; }
    [NotMapped]
    public string? PhoneNumber { get; set; }
    public Address? Address { get; set; }
    private readonly List<RefreshToken> _refreshTokens = new List<RefreshToken>();
    [JsonIgnore] // Never serialize refresh tokens (even hashed) into API responses or logs.
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
    [SetsRequiredMembers]
    public User()
    { /* Required by EF */
        IdentityId = string.Empty;
        FirstName = string.Empty;
        LastName = string.Empty;
        UserName = string.Empty;
        Email = string.Empty;
    }
    [SetsRequiredMembers]
    public User(string firstName, string lastName, string identityId, string userName, string email, string? phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        IdentityId = identityId;
        UserName = userName;
        Email = email;
        PhoneNumber = phoneNumber;
    }
    // Refresh tokens are persisted as SHA-512 hashes (RefreshToken.Token holds the hash). Callers always pass the raw token.
    public bool HasValidRefreshToken(string refreshToken) => _refreshTokens.Any(rt => rt.Active && RefreshTokenHasher.Matches(refreshToken, rt.Token));
    public void AddRefreshToken(string token, string? remoteIpAddress, double daysToExpire = 5) =>
        _refreshTokens.Add(new RefreshToken(RefreshTokenHasher.Hash(token), DateTimeOffset.UtcNow.AddDays(daysToExpire), Id, remoteIpAddress));
    public void RemoveRefreshToken(string refreshToken)
    {
        RefreshToken? token = _refreshTokens.FirstOrDefault(t => RefreshTokenHasher.Matches(refreshToken, t.Token));
        if (token != null)
            _refreshTokens.Remove(token);
    }
    /// <summary>
    /// Revokes every refresh token of this user. Used when a refresh token that is not (or no longer) valid is presented
    /// together with a genuinely signed access token, which indicates the token may have been stolen and replayed.
    /// </summary>
    public void RevokeAllRefreshTokens() => _refreshTokens.Clear();
}