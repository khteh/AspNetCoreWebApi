using System;
using System.Threading.Tasks;
using Moq;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.UseCases;
using Xunit;
namespace Web.Api.Core.UnitTests.UseCases;

public class GenerateChangeEmailTokenUseCaseUnitTests
{
    [Fact]
    public async Task Handle_GenerateChangeEmailToken_ShouldSucceed()
    {
        // arrange

        // 1. We need to store the user data somehow
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository
              .Setup(repo => repo.GenerateChangeEmailToken(It.IsAny<string>(), It.IsAny<string>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.CodeResponse(string.Empty, string.Empty, true));

        // 2. The use case and star of this test
        var useCase = new GenerateChangeEmailTokenUseCase(mockUserRepository.Object);

        // 3. The output port is the mechanism to pass response data from the use case to a Presenter
        // for final preparation to deliver back to the UI/web page/api response etc.
        var mockOutputPort = new Mock<IOutputPort<CodeResponse>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<CodeResponse>()));

        // act

        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        var response = await useCase.Handle(new GenerateChangeEmailTokenRequest("id", "me@email.com"), mockOutputPort.Object);

        // assert
        Assert.True(response);
        mockUserRepository.VerifyAll();
        mockOutputPort.VerifyAll();
    }
    [Fact]
    public async Task Handle_GenerateChangeEmailToken_InvalidEmail_ShouldFail()
    {
        // arrange

        // 1. We need to store the user data somehow
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository
              .Setup(repo => repo.GenerateChangeEmailToken(It.IsAny<string>(), It.IsAny<string>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.CodeResponse(string.Empty, string.Empty, false));

        // 2. The use case and star of this test
        var useCase = new GenerateChangeEmailTokenUseCase(mockUserRepository.Object);

        // 3. The output port is the mechanism to pass response data from the use case to a Presenter
        // for final preparation to deliver back to the UI/web page/api response etc.
        var mockOutputPort = new Mock<IOutputPort<CodeResponse>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<CodeResponse>()));

        // act

        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        var response = await useCase.Handle(new GenerateChangeEmailTokenRequest("id", "Hello World!!!"), mockOutputPort.Object);

        // assert
        Assert.False(response);
        mockUserRepository.Verify(factory => factory.GenerateChangeEmailToken(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockOutputPort.VerifyAll();
    }
}