using System;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.Services;
using Web.Api.Core.UseCases;
using Xunit;
namespace Web.Api.Core.UnitTests.UseCases;
public class ExchangeRefreshTokenUseCaseUnitTests
{
    [Fact]
    public async void Handle_ExchangeRefreshToken_ShouldSucceed()
    {
        // arrange

        // 1. We need to store the user data somehow
        // AccessToken jwtToken = await _jwtFactory.GenerateEncodedToken(user.IdentityId, user.UserName);
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository
              .Setup(repo => repo.ExchangeRefreshToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.ExchangeRefreshTokenResponse(new AccessToken(string.Empty, -1), string.Empty, true));

        // 2. The use case and star of this test
        var useCase = new ExchangeRefreshTokenUseCase(mockUserRepository.Object);

        // 3. The output port is the mechanism to pass response data from the use case to a Presenter
        // for final preparation to deliver back to the UI/web page/api response etc.
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act

        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        var response = await useCase.Handle(new ExchangeRefreshTokenRequest("accessToken", "refreshToken", "signingKey"), mockOutputPort.Object);

        // assert
        Assert.True(response);
        mockUserRepository.VerifyAll();
        mockOutputPort.VerifyAll();
    }
}