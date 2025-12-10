using System;
using System.Threading.Tasks;
using Moq;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.UseCases;
using Xunit;
namespace Web.Api.Core.UnitTests.UseCases;

public class ForgotPasswordUseCaseUnitTests
{
    [Fact]
    public async Task Handle_ForgotPassword_InvalidEmail_ShouldFail()
    {
        // arrange

        // 1. We need to store the user data somehow
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository
              .Setup(repo => repo.ForgotPassword(It.IsAny<string>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.CodeResponse(Guid.Empty, string.Empty, false));

        // 2. The use case and star of this test
        var useCase = new ForgotPasswordUseCase(mockUserRepository.Object);

        // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
        // for final preparation to deliver back to the UI/web page/api response etc.
        var mockOutputPort = new Mock<IOutputPort<CodeResponse>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<CodeResponse>()));

        // act

        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        var response = await useCase.Handle(new ForgotPasswordRequest("Hello World!!!"), mockOutputPort.Object);

        // assert
        Assert.False(response);
        mockUserRepository.Verify(factory => factory.ForgotPassword(It.IsAny<string>()), Times.Never);
        mockOutputPort.VerifyAll();
    }
    [Fact]
    public async Task Handle_ForgotPassword_ValidEmail_ShouldSucceed()
    {
        // arrange

        // 1. We need to store the user data somehow
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository
              .Setup(repo => repo.ForgotPassword(It.IsAny<string>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.CodeResponse(Guid.Empty, string.Empty, true));

        // 2. The use case and star of this test
        var useCase = new ForgotPasswordUseCase(mockUserRepository.Object);

        // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
        // for final preparation to deliver back to the UI/web page/api response etc.
        var mockOutputPort = new Mock<IOutputPort<CodeResponse>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<CodeResponse>()));

        // act

        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        var response = await useCase.Handle(new ForgotPasswordRequest("me@email.com"), mockOutputPort.Object);

        // assert
        Assert.True(response);
        mockUserRepository.VerifyAll();
        mockOutputPort.VerifyAll();
    }
}