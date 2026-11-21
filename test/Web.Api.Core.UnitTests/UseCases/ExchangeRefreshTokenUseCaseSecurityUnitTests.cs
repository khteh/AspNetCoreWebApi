using System.Linq;
using System.Threading.Tasks;
using Moq;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.UseCases;
using Xunit;
namespace Web.Api.Core.UnitTests.UseCases;

/// <summary>
/// The refresh token endpoints are anonymous, so error responses must never echo tokens or the JWT signing key.
/// </summary>
public class ExchangeRefreshTokenUseCaseSecurityUnitTests
{
    private const string SigningKey = "THIS-IS-THE-SERVER-SIGNING-KEY-THAT-MUST-NEVER-BE-RETURNED-TO-ANY-CALLER";
    [Theory]
    [InlineData("", "refresh-token-value")]
    [InlineData("access-token-value", "")]
    [InlineData("", "")]
    public async Task Handle_GivenMissingTokens_FailsWithoutEchoingInputsOrSigningKey(string accessToken, string refreshToken)
    {
        var mockUserRepository = new Mock<IUserRepository>(MockBehavior.Strict); // must not be called
        ExchangeRefreshTokenResponse? captured = null;
        var mockOutputPort = new Mock<IOutputPort<ExchangeRefreshTokenResponse>>();
        mockOutputPort.Setup(o => o.Handle(It.IsAny<ExchangeRefreshTokenResponse>()))
                      .Callback<ExchangeRefreshTokenResponse>(r => captured = r)
                      .Returns(Task.CompletedTask);
        var useCase = new ExchangeRefreshTokenUseCase(mockUserRepository.Object);

        bool result = await useCase.Handle(new ExchangeRefreshTokenRequest(accessToken, refreshToken, SigningKey), mockOutputPort.Object);

        Assert.False(result);
        Assert.NotNull(captured);
        Assert.False(captured!.Success);
        Assert.NotNull(captured.Errors);
        string allErrorText = string.Join(" ", captured.Errors!.Select(e => $"{e.Code} {e.Description}"));
        Assert.DoesNotContain(SigningKey, allErrorText);
        if (!string.IsNullOrEmpty(accessToken))
            Assert.DoesNotContain(accessToken, allErrorText);
        if (!string.IsNullOrEmpty(refreshToken))
            Assert.DoesNotContain(refreshToken, allErrorText);
    }
}
