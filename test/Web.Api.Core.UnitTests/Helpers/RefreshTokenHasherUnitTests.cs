using System;
using Web.Api.Core.Helpers;
using Xunit;
namespace Web.Api.Core.UnitTests.Helpers;

public class RefreshTokenHasherUnitTests
{
    [Fact]
    public void Hash_IsDeterministicAndNotTheRawToken()
    {
        const string token = "cvVsJXuuvb+gTyz+Rk0mBbitkw3AaLgsLecU3cwsUXU=";
        string hash1 = RefreshTokenHasher.Hash(token);
        string hash2 = RefreshTokenHasher.Hash(token);
        Assert.Equal(hash1, hash2);
        Assert.NotEqual(token, hash1);
        Assert.Equal(88, hash1.Length); // Base64 of a 32-byte SHA-512 digest
    }
    [Fact]
    public void Hash_DifferentTokensProduceDifferentHashes() =>
        Assert.NotEqual(RefreshTokenHasher.Hash("token-a"), RefreshTokenHasher.Hash("token-b"));
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Hash_RejectsNullOrEmpty(string? token) =>
        Assert.ThrowsAny<ArgumentException>(() => RefreshTokenHasher.Hash(token!));
    [Fact]
    public void Matches_ReturnsTrueOnlyForTheOriginalToken()
    {
        string hash = RefreshTokenHasher.Hash("secret-refresh-token");
        Assert.True(RefreshTokenHasher.Matches("secret-refresh-token", hash));
        Assert.False(RefreshTokenHasher.Matches("other-token", hash));
        Assert.False(RefreshTokenHasher.Matches(hash, hash)); // presenting the stored hash itself must not work
    }
    [Theory]
    [InlineData(null, "hash")]
    [InlineData("", "hash")]
    [InlineData("token", null)]
    [InlineData("token", "")]
    public void Matches_ReturnsFalseForMissingValues(string? token, string? hash) =>
        Assert.False(RefreshTokenHasher.Matches(token!, hash!));
}
