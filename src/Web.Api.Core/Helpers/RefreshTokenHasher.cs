using System;
using System.Security.Cryptography;
using System.Text;
namespace Web.Api.Core.Helpers;

/// <summary>
/// Refresh tokens are stored as SHA-256 hashes so that a database leak does not hand out usable tokens.
/// The raw token is only ever returned to the client that requested it.
/// Refresh tokens are 32 random bytes from a CSPRNG, so an unsalted fast hash is sufficient (no dictionary to attack).
/// </summary>
public static class RefreshTokenHasher
{
    public static string Hash(string refreshToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(refreshToken);
        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
    }
    /// <summary>
    /// Constant-time comparison of a raw refresh token against a stored hash.
    /// </summary>
    public static bool Matches(string refreshToken, string storedHash)
    {
        if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(storedHash))
            return false;
        return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(Hash(refreshToken)), Encoding.UTF8.GetBytes(storedHash));
    }
}
