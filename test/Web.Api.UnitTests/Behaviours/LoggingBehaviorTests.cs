using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Web.Api.Behaviours;
using Web.Api.Commands;
using Web.Api.Core.DTO;
using Web.Api.Models.Response;
using Xunit;
namespace Web.Api.UnitTests.Behaviours;

public class LoggingBehaviorTests
{
    private const string Password = "Sup3r-S3cret-P@ssw0rd";
    private const string AccessTokenValue = "eyJhbGciOiJIUzUxMiJ9.payload.signature";
    private const string RefreshTokenValue = "cvVsJXuuvb+gTyz+Rk0mBbitkw3AaLgsLecU3cwsUXU=";

    private sealed class ListLogger<T> : ILogger<T>
    {
        public List<string> Messages { get; } = new();
        public IDisposable BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) =>
            Messages.Add(formatter(state, exception));
    }

    [Fact]
    public async Task LogInCommand_PasswordAndTokensAreNotLogged()
    {
        var logger = new ListLogger<LoggingBehavior<LogInCommand, LogInResponse>>();
        var behavior = new LoggingBehavior<LogInCommand, LogInResponse>(logger);
        var response = new LogInResponse(new AccessToken(AccessTokenValue, 7200), RefreshTokenValue, true, new List<Error>());

        await behavior.Handle(new LogInCommand("testuser", Password, "127.0.0.1"), _ => Task.FromResult(response), CancellationToken.None);

        Assert.NotEmpty(logger.Messages);
        string all = string.Join(Environment.NewLine, logger.Messages);
        Assert.Contains("testuser", all); // non-sensitive fields are still useful in logs
        Assert.DoesNotContain(Password, all);
        Assert.DoesNotContain(AccessTokenValue, all);
        Assert.DoesNotContain(RefreshTokenValue, all);
    }
    [Fact]
    public async Task ExchangeRefreshTokenCommand_TokensAreNotLogged()
    {
        var logger = new ListLogger<LoggingBehavior<ExchangeRefreshTokenCommand, ExchangeRefreshTokenResponse>>();
        var behavior = new LoggingBehavior<ExchangeRefreshTokenCommand, ExchangeRefreshTokenResponse>(logger);
        var response = new ExchangeRefreshTokenResponse(new AccessToken(AccessTokenValue, 7200), "new-" + RefreshTokenValue, true, new List<Error>());

        await behavior.Handle(new ExchangeRefreshTokenCommand(AccessTokenValue, RefreshTokenValue), _ => Task.FromResult(response), CancellationToken.None);

        string all = string.Join(Environment.NewLine, logger.Messages);
        Assert.DoesNotContain(AccessTokenValue, all);
        Assert.DoesNotContain(RefreshTokenValue, all);
    }
    [Theory]
    [InlineData("Password", true)]
    [InlineData("NewPassword", true)]
    [InlineData("AccessToken", true)]
    [InlineData("RefreshToken", true)]
    [InlineData("SigningKey", true)]
    [InlineData("ClientSecret", true)]
    [InlineData("UserName", false)]
    [InlineData("Email", false)]
    [InlineData("RemoteIpAddress", false)]
    public void LogSanitizer_IsSensitive(string memberName, bool expected) =>
        Assert.Equal(expected, LogSanitizer.IsSensitive(memberName));
    [Fact]
    public void LogSanitizer_Describe_MasksSensitiveMembers()
    {
        string description = LogSanitizer.Describe(new LogInCommand("testuser", Password, "127.0.0.1"));
        Assert.Contains("UserName = testuser", description);
        Assert.Contains($"Password = {LogSanitizer.Mask}", description);
        Assert.DoesNotContain(Password, description);
    }
}
