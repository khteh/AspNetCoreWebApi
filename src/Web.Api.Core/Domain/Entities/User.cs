using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Web.Api.Core.Shared;
namespace Web.Api.Core.Domain.Entities;
[Serializable]
public class User : BaseEntity
{
    public string IdentityId { get; set; }
    [NotMapped]
    public string FirstName { get; set; } // EF migrations require at least initter - won't work on auto-property
    [NotMapped]
    public string LastName { get; set; }
    [NotMapped]
    public string UserName { get; set; } // Required by automapper
    [NotMapped]
    public string Email { get; set; }
    [NotMapped]
    public string PhoneNumber { get; set; }
    [Required]
    public Address Address { get; set; }
    private readonly List<RefreshToken> _refreshTokens = new List<RefreshToken>();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
    public User() { /* Required by EF */ }
    public User(string firstName, string lastName, string identityId, string userName, string email, string phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        IdentityId = identityId;
        UserName = userName;
        Email = email;
        PhoneNumber = phoneNumber;
    }
    public bool HasValidRefreshToken(string refreshToken) => _refreshTokens.Any(rt => rt.Token == refreshToken && rt.Active);
    public void AddRefreshToken(string token, string remoteIpAddress, double daysToExpire = 5) =>
        _refreshTokens.Add(new RefreshToken(token, DateTimeOffset.UtcNow.AddDays(daysToExpire), Id, remoteIpAddress));
    public void RemoveRefreshToken(string refreshToken) =>
        _refreshTokens.Remove(_refreshTokens.First(t => t.Token == refreshToken));
}