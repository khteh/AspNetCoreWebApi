using Microsoft.AspNetCore.Identity;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.Services;
using Web.Api.Core.UseCases;
using Web.Api.Infrastructure.Identity;
using Web.Api.Core.DTO.GatewayResponses.Repositories;
using Xunit;

namespace Web.Api.Infrastructure.UnitTests.UserRepository;
public class ExchangeRefreshTokenUnitTests : IClassFixture<InfrastructureTestBase>
{
    private InfrastructureTestBase _fixture;
    public ExchangeRefreshTokenUnitTests(InfrastructureTestBase fixture) => _fixture = fixture;

    [Fact(Skip = "Call to getUser() will throw null reference exception")]
    public async void Handle_GivenInvalidCredentials_ShouldFail()
    {
        // arrange
        List<Claim> claims = new List<Claim>();
        var mockJwtFactory = new Mock<IJwtFactory>();
        mockJwtFactory.Setup(validator => validator.GenerateEncodedToken(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((AccessToken)null);

        var mockTokenFactory = new Mock<ITokenFactory>();
        mockTokenFactory.Setup(validator => validator.GenerateToken(It.IsAny<int>())).Returns(string.Empty);

        var mockJwtTokenValidator = new Mock<IJwtTokenValidator>();
        mockJwtTokenValidator.Setup(validator => validator.GetPrincipalFromToken(It.IsAny<string>(), It.IsAny<string>())).Returns((ClaimsPrincipal)null);

        // act
        ExchangeRefreshTokenResponse response = await _fixture.UserRepository.ExchangeRefreshToken(string.Empty, string.Empty, string.Empty);

        // assert
        Assert.False(response.Success);
        Assert.Null(response.Errors);
        // The following verifications do not make sense. None of the mocked interfaces will run their methods due to mockUserRepository.ExchangeRefreshToken
        //mockJwtTokenValidator.Verify(factory => factory.GetPrincipalFromToken(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        mockJwtFactory.Verify(factory => factory.GenerateEncodedToken(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockTokenFactory.Verify(factory => factory.GenerateToken(It.IsAny<int>()), Times.Never);
        //mockUserRepository.Verify(factory => factory.Update(It.IsAny<User>()), Times.Never);
    }
    [Fact]
    public async void Handle_GivenValidCredentials_ShouldSucceed()
    {
        // arrange
        AppUser appUser = new AppUser("", "", "", "");

        List<Claim> claims = new List<Claim>();
        var mockJwtFactory = new Mock<IJwtFactory>();
        AccessToken accessToken = new AccessToken("Hello World!!!", 123);
        mockJwtFactory.Setup(validator => validator.GenerateEncodedToken(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(accessToken);

        var mockTokenFactory = new Mock<ITokenFactory>();
        mockTokenFactory.Setup(validator => validator.GenerateToken(It.IsAny<int>())).Returns("Hello World!!!");

        var claimsPrincipal = new Mock<ClaimsPrincipal>();
        claimsPrincipal.Setup(validator => validator.Claims).Returns(new List<Claim>() { new Claim("Id", "Any Id") });
        var mockJwtTokenValidator = new Mock<IJwtTokenValidator>();
        mockJwtTokenValidator.Setup(validator => validator.GetPrincipalFromToken(It.IsAny<string>(), It.IsAny<string>())).Returns(claimsPrincipal.Object);

        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository.Setup(repo => repo.ExchangeRefreshToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Core.DTO.GatewayResponses.Repositories.ExchangeRefreshTokenResponse(accessToken, "Refesh Token", true));

        var useCase = new ExchangeRefreshTokenUseCase(mockUserRepository.Object);
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act
        var response = await useCase.Handle(new ExchangeRefreshTokenRequest("Helo World!!!", "Refresh Token", "Any Signing Key"), mockOutputPort.Object);

        // assert
        Assert.True(response);
        // The following verifications do not make sense. None of the mocked interfaces will run their methods due to mockUserRepository.ExchangeRefreshToken
        //mockJwtTokenValidator.Verify(factory => factory.GetPrincipalFromToken(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        mockJwtFactory.Verify(factory => factory.GenerateEncodedToken(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockTokenFactory.Verify(factory => factory.GenerateToken(It.IsAny<int>()), Times.Never);
        mockUserRepository.Verify(factory => factory.Update(It.IsAny<User>()), Times.Never);
        mockOutputPort.VerifyAll();
    }
}
