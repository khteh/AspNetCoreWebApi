using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
    [SetsRequiredMembers]
    public User() { /* Required by EF */ }
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
    public bool HasValidRefreshToken(string refreshToken) => _refreshTokens.Any(rt => rt.Token == refreshToken && rt.Active);
    public void AddRefreshToken(string token, string? remoteIpAddress, double daysToExpire = 5) =>
        _refreshTokens.Add(new RefreshToken(token, DateTimeOffset.UtcNow.AddDays(daysToExpire), Id, remoteIpAddress));
    public void RemoveRefreshToken(string refreshToken) =>
        _refreshTokens.Remove(_refreshTokens.First(t => t.Token == refreshToken));
}