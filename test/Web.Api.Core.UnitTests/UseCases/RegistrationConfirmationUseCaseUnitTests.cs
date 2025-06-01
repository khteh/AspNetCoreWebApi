using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Api.Core.DTO.GatewayResponses.Repositories;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.UseCases;
using Xunit;
namespace Web.Api.Core.UnitTests.UseCases;

public class RegistrationConfirmationUseCaseUnitTests
{
    [Fact]
    public async Task Handle_GivenValidRegistrationDetails_ShouldSucceed()
    {
        // arrange

        // 1. We need to store the user data somehow
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository
              .Setup(repo => repo.RegistrationConfirmation(It.IsAny<string>()))
              .ReturnsAsync(new CodeResponse(string.Empty, string.Empty, true));

        var mockLogger = new Mock<ILogger<RegistrationConfirmationUseCase>>();
        // 2. The use case and star of this test
        var useCase = new RegistrationConfirmationUseCase(mockLogger.Object, mockUserRepository.Object);

        // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
        // for final preparation to deliver back to the UI/web page/api response etc.
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act

        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        var response = await useCase.Handle(new RegistrationConfirmationRequest("me@email.com"), mockOutputPort.Object);

        // assert
        Assert.True(response);
        mockUserRepository.VerifyAll();
        mockOutputPort.VerifyAll();
    }
    [Fact]
    public async Task Handle_GivenInValidRegistrationDetails_ShouldFail()
    {
        // arrange

        // 1. We need to store the user data somehow
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository
              .Setup(repo => repo.RegistrationConfirmation(It.IsAny<string>()))
              .ReturnsAsync(new CodeResponse(string.Empty, string.Empty, true));

        var mockLogger = new Mock<ILogger<RegistrationConfirmationUseCase>>();
        // 2. The use case and star of this test
        var useCase = new RegistrationConfirmationUseCase(mockLogger.Object, mockUserRepository.Object);

        // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
        // for final preparation to deliver back to the UI/web page/api response etc.
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act

        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        var response = await useCase.Handle(new RegistrationConfirmationRequest("email"), mockOutputPort.Object);

        // assert
        Assert.False(response);
        mockUserRepository.Verify(factory => factory.RegistrationConfirmation(It.IsAny<string>()), Times.Never);
        mockOutputPort.VerifyAll();
    }
}