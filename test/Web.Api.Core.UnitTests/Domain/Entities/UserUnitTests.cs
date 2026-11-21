using System;
using System.Linq;
using System.Text.Json;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.Helpers;
using Xunit;
namespace Web.Api.Core.UnitTests.Domain.Entities;
public class UserUnitTests
{
    [Fact]
    public void HasValidRefreshToken_GivenValidToken_ShouldReturnTrue()
    {
        // arrange
        const string refreshToken = "1234";
        var user = new User("", "", "", "", "", "");
        user.AddRefreshToken(refreshToken, "127.0.0.1");

        // act
        var result = user.HasValidRefreshToken(refreshToken);

        Assert.True(result);
    }
    [Fact]
    public void HasValidRefreshToken_GivenExpiredToken_ShouldReturnFalse()
    {
        // arrange
        const string refreshToken = "1234";
        var user = new User("", "", "", "", "", "");
        user.AddRefreshToken(refreshToken, "127.0.0.1", -6); // Provision with token 6 days old

        // act
        var result = user.HasValidRefreshToken(refreshToken);

        Assert.False(result);
    }
    [Fact]
    public void AddRefreshToken_StoresHashNotRawToken()
    {
        const string refreshToken = "cvVsJXuuvb+gTyz+Rk0mBbitkw3AaLgsLecU3cwsUXU=";
        var user = new User("", "", "", "", "", "");
        user.AddRefreshToken(refreshToken, "127.0.0.1");

        RefreshToken stored = Assert.Single(user.RefreshTokens);
        Assert.NotEqual(refreshToken, stored.Token);
        Assert.Equal(RefreshTokenHasher.Hash(refreshToken), stored.Token);
    }
    [Fact]
    public void HasValidRefreshToken_GivenUnknownToken_ShouldReturnFalse()
    {
        var user = new User("", "", "", "", "", "");
        user.AddRefreshToken("1234", "127.0.0.1");
        Assert.False(user.HasValidRefreshToken("5678"));
    }
    [Fact]
    public void HasValidRefreshToken_GivenStoredHash_ShouldReturnFalse()
    {
        // A leaked database row (the hash) must not be usable as a refresh token.
        var user = new User("", "", "", "", "", "");
        user.AddRefreshToken("1234", "127.0.0.1");
        Assert.False(user.HasValidRefreshToken(user.RefreshTokens.Single().Token));
    }
    [Fact]
    public void RemoveRefreshToken_RemovesOnlyThatToken()
    {
        var user = new User("", "", "", "", "", "");
        user.AddRefreshToken("old", "127.0.0.1");
        user.AddRefreshToken("other", "127.0.0.1");

        user.RemoveRefreshToken("old");

        Assert.False(user.HasValidRefreshToken("old"));
        Assert.True(user.HasValidRefreshToken("other"));
        Assert.Single(user.RefreshTokens);
    }
    [Fact]
    public void RemoveRefreshToken_GivenUnknownToken_DoesNotThrow()
    {
        var user = new User("", "", "", "", "", "");
        user.AddRefreshToken("1234", "127.0.0.1");
        user.RemoveRefreshToken("unknown");
        Assert.Single(user.RefreshTokens);
    }
    [Fact]
    public void RevokeAllRefreshTokens_InvalidatesEveryToken()
    {
        var user = new User("", "", "", "", "", "");
        user.AddRefreshToken("a", "127.0.0.1");
        user.AddRefreshToken("b", "127.0.0.1");

        user.RevokeAllRefreshTokens();

        Assert.Empty(user.RefreshTokens);
        Assert.False(user.HasValidRefreshToken("a"));
        Assert.False(user.HasValidRefreshToken("b"));
    }
    [Fact]
    public void RefreshTokens_AreNotSerialized()
    {
        var user = new User("First", "Last", "identity-id", "username", "user@email.com", null);
        user.AddRefreshToken("secret-refresh-token", "127.0.0.1");

        string json = JsonSerializer.Serialize(user);

        Assert.DoesNotContain("refreshTokens", json, StringComparison.OrdinalIgnoreCase);
    }
}