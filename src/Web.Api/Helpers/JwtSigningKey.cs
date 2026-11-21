using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Web.Api.Infrastructure.Auth;
namespace Web.Api.Helpers;

/// <summary>
/// Resolves the HMAC-SHA512 key used to sign and validate JWTs.
/// - Configured key (environment variable AuthSettings__SecretKey, user secrets, a secret store, appsettings.{env}.json) always wins.
/// - Development only: when nothing is configured, a random key is generated once per machine, stored outside the repository and reused.
/// - Any other environment: startup fails if the key is missing or too short. Never commit a real key to source control.
/// </summary>
public static class JwtSigningKey
{
    public const int MinKeyBytes = 64; // HMAC-SHA512 requires a key of at least 512 bits
    public static readonly string ConfigurationKey = $"{nameof(AuthSettings)}:{nameof(AuthSettings.SecretKey)}";
    public static string DefaultDevelopmentKeyPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AspNetCoreWebApi", "jwt-signing-key");

    public static string GetOrCreate(IConfiguration configuration, IHostEnvironment environment, string? developmentKeyPath = null)
    {
        string? key = configuration[ConfigurationKey];
        if (!string.IsNullOrWhiteSpace(key))
        {
            if (Encoding.UTF8.GetByteCount(key) < MinKeyBytes)
                throw new InvalidOperationException($"{ConfigurationKey} must be at least {MinKeyBytes} bytes.");
            return key;
        }
        if (!environment.IsDevelopment())
            throw new InvalidOperationException($"{ConfigurationKey} is not configured. Set the AuthSettings__SecretKey environment variable or use a secret store.");

        string path = developmentKeyPath ?? DefaultDevelopmentKeyPath;
        if (File.Exists(path))
        {
            key = File.ReadAllText(path).Trim();
            if (Encoding.UTF8.GetByteCount(key) >= MinKeyBytes)
                return key;
        }
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(MinKeyBytes));
        File.WriteAllText(path, key);
        if (!OperatingSystem.IsWindows())
            File.SetUnixFileMode(path, UnixFileMode.UserRead | UnixFileMode.UserWrite); // chmod 600
        return key;
    }
}
