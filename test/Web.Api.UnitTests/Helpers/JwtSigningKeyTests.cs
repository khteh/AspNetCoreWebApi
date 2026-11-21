using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Moq;
using Web.Api.Helpers;
using Xunit;
namespace Web.Api.UnitTests.Helpers;

public sealed class JwtSigningKeyTests : IDisposable
{
    private readonly string _tempDir = Path.Combine(Path.GetTempPath(), "JwtSigningKeyTests", Guid.NewGuid().ToString("N"));
    private string KeyPath => Path.Combine(_tempDir, "jwt-signing-key");
    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }
    private static IConfiguration Config(string key) => new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string> { [JwtSigningKey.ConfigurationKey] = key })
        .Build();
    private static IHostEnvironment Env(string name)
    {
        var env = new Mock<IHostEnvironment>();
        env.SetupGet(e => e.EnvironmentName).Returns(name);
        return env.Object;
    }

    [Theory]
    [InlineData("Development")]
    [InlineData("Production")]
    public void GetOrCreate_ConfiguredKey_IsReturnedAsIs(string environment)
    {
        string configured = new string('k', JwtSigningKey.MinKeyBytes);
        Assert.Equal(configured, JwtSigningKey.GetOrCreate(Config(configured), Env(environment), KeyPath));
        Assert.False(File.Exists(KeyPath));
    }
    [Theory]
    [InlineData("Development")]
    [InlineData("Production")]
    public void GetOrCreate_ConfiguredKeyTooShort_Throws(string environment)
    {
        string configured = new string('k', JwtSigningKey.MinKeyBytes - 1);
        Assert.Throws<InvalidOperationException>(() => JwtSigningKey.GetOrCreate(Config(configured), Env(environment), KeyPath));
    }
    [Theory]
    [InlineData("Production")]
    [InlineData("Staging")]
    [InlineData("IntegrationTests")]
    public void GetOrCreate_MissingKeyOutsideDevelopment_Throws(string environment)
    {
        Assert.Throws<InvalidOperationException>(() => JwtSigningKey.GetOrCreate(Config(null), Env(environment), KeyPath));
        Assert.False(File.Exists(KeyPath));
    }
    [Fact]
    public void GetOrCreate_MissingKeyInDevelopment_GeneratesOnceAndReuses()
    {
        string first = JwtSigningKey.GetOrCreate(Config(""), Env("Development"), KeyPath);
        string second = JwtSigningKey.GetOrCreate(Config(""), Env("Development"), KeyPath);

        Assert.True(Encoding.UTF8.GetByteCount(first) >= JwtSigningKey.MinKeyBytes);
        Assert.Equal(first, second);
        Assert.Equal(first, File.ReadAllText(KeyPath));
        Assert.Equal(JwtSigningKey.MinKeyBytes, Convert.FromBase64String(first).Length);
        if (!OperatingSystem.IsWindows())
            Assert.Equal(UnixFileMode.UserRead | UnixFileMode.UserWrite, File.GetUnixFileMode(KeyPath));
    }
    [Fact]
    public void GetOrCreate_DevelopmentKeyFileTooShort_IsRegenerated()
    {
        Directory.CreateDirectory(_tempDir);
        File.WriteAllText(KeyPath, "short");

        string key = JwtSigningKey.GetOrCreate(Config(null), Env("Development"), KeyPath);

        Assert.True(Encoding.UTF8.GetByteCount(key) >= JwtSigningKey.MinKeyBytes);
        Assert.Equal(key, File.ReadAllText(KeyPath));
    }
}
