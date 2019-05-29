using System;
using Web.Api.Core.Shared;


namespace Web.Api.Core.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; private set; }
        public DateTimeOffset Expires { get; private set; }
        public int UserId { get; private set; }
        public bool Active => DateTimeOffset.UtcNow <= Expires;
        public string RemoteIpAddress { get; private set; }
         public RefreshToken(string token, DateTimeOffset expires, int userId,string remoteIpAddress)
        {
            Token = token;
            Expires = expires;
            UserId = userId;
            RemoteIpAddress = remoteIpAddress;
        }
    }
}