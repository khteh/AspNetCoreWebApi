using System;
using System.Collections.Generic;
using System.Linq;
using Web.Api.Core.Shared;

namespace Web.Api.Core.Domain.Entities
{
    [Serializable]
    public class User : BaseEntity
    {
        public string FirstName { get; init; } // EF migrations require at least initter - won't work on auto-property
        public string LastName { get; init; }
        public string IdentityId { get; init; }
        public string UserName { get; init; } // Required by automapper
        public string Email { get; init; }
        private readonly List<RefreshToken> _refreshTokens = new List<RefreshToken>();
        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
        public User() { /* Required by EF */ }
        public User(string firstName, string lastName, string identityId, string userName)
        {
            FirstName = firstName;
            LastName = lastName;
            IdentityId = identityId;
            UserName = userName;
        }
        public bool HasValidRefreshToken(string refreshToken) => _refreshTokens.Any(rt => rt.Token == refreshToken && rt.Active);
        public void AddRefreshToken(string token, string remoteIpAddress,double daysToExpire=5) =>
            _refreshTokens.Add(new RefreshToken(token, DateTimeOffset.UtcNow.AddDays(daysToExpire), Id, remoteIpAddress));
        public void RemoveRefreshToken(string refreshToken) =>
            _refreshTokens.Remove(_refreshTokens.First(t => t.Token == refreshToken));
    }
}