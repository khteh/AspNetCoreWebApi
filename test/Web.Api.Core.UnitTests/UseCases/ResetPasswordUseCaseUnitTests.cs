using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.UseCases;
using Xunit;
namespace Web.Api.Core.UnitTests.UseCases;

public class ResetPasswordUseCaseUnitTests
{
    /*
        if (!string.IsNullOrEmpty(request.Id))
            response = await _userRepository.ResetPassword(request.Id, request.NewPassword);
        else if (!string.IsNullOrEmpty(request.Email) && EmailValidation.IsValidEmail(request.Email) && !string.IsNullOrEmpty(request.Code))
            response = await _userRepository.ResetPassword(request.Email, request.NewPassword, request.Code);
    */
    [Fact]
    public async Task Handle_ResetPasswordUsingId_ShouldSucceed()
    {
        // arrange

        // 1. We need to store the user data somehow
        var mockLogger = new Mock<ILogger<ResetPasswordUseCase>>();
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository
              .Setup(repo => repo.ResetPassword(It.IsAny<string>(), It.IsAny<string>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.PasswordResponse("", true));

        // 2. The use case and star of this test
        var useCase = new ResetPasswordUseCase(mockLogger.Object, mockUserRepository.Object);

        // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
        // for final preparation to deliver back to the UI/web page/api response etc.
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act

        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        var response = await useCase.Handle(new ResetPasswordRequest("id", null, "newPassword", null), mockOutputPort.Object);

        // assert
        Assert.True(response);
        mockUserRepository.VerifyAll();
        mockOutputPort.VerifyAll();
    }
    [Fact]
    public async Task Handle_ResetPasswordUsingId_InvalidPassword_ShouldFail()
    {
        // arrange

        // 1. We need to store the user data somehow
        var mockLogger = new Mock<ILogger<ResetPasswordUseCase>>();
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository
              .Setup(repo => repo.ResetPassword(It.IsAny<string>(), It.IsAny<string>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.PasswordResponse("", true));

        // 2. The use case and star of this test
        var useCase = new ResetPasswordUseCase(mockLogger.Object, mockUserRepository.Object);

        // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
        // for final preparation to deliver back to the UI/web page/api response etc.
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act

        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        var response = await useCase.Handle(new ResetPasswordRequest("id", null, string.Empty, null), mockOutputPort.Object);

        // assert
        Assert.False(response);
        mockUserRepository.Verify(factory => factory.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockOutputPort.VerifyAll();
    }
    [Fact]
    public async Task Handle_ResetPasswordUsingGeneratedCode_ShouldSucceed()
    {
        // arrange

        // 1. We need to store the user data somehow
        var mockLogger = new Mock<ILogger<ResetPasswordUseCase>>();
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository
              .Setup(repo => repo.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.PasswordResponse("", true));

        // 2. The use case and star of this test
        var useCase = new ResetPasswordUseCase(mockLogger.Object, mockUserRepository.Object);

        // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
        // for final preparation to deliver back to the UI/web page/api response etc.
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act

        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        var response = await useCase.Handle(new ResetPasswordRequest(null, "me@email.com", "newPassword", "code"), mockOutputPort.Object);

        // assert
        Assert.True(response);
        mockUserRepository.VerifyAll();
        mockOutputPort.VerifyAll();
    }
    [Fact]
    public async Task Handle_ResetPasswordUsingGeneratedCode_InvalidEmail_ShouldFail()
    {
        // arrange

        // 1. We need to store the user data somehow
        var mockLogger = new Mock<ILogger<ResetPasswordUseCase>>();
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository
              .Setup(repo => repo.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.PasswordResponse("", true));

        // 2. The use case and star of this test
        var useCase = new ResetPasswordUseCase(mockLogger.Object, mockUserRepository.Object);

        // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
        // for final preparation to deliver back to the UI/web page/api response etc.
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act

        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        var response = await useCase.Handle(new ResetPasswordRequest(null, "email", "newPassword", "code"), mockOutputPort.Object);

        // assert
        Assert.False(response);
        mockUserRepository.Verify(factory => factory.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockOutputPort.VerifyAll();
    }
    [Fact]
    public async Task Handle_ResetPasswordUsingGeneratedCode_InvalidCode_ShouldFail()
    {
        // arrange

        // 1. We need to store the user data somehow
        var mockLogger = new Mock<ILogger<ResetPasswordUseCase>>();
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository
              .Setup(repo => repo.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.PasswordResponse("", true));

        // 2. The use case and star of this test
        var useCase = new ResetPasswordUseCase(mockLogger.Object, mockUserRepository.Object);

        // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
        // for final preparation to deliver back to the UI/web page/api response etc.
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act

        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        var response = await useCase.Handle(new ResetPasswordRequest(null, "me@email.com", "newPassword", string.Empty), mockOutputPort.Object);

        // assert
        Assert.False(response);
        mockUserRepository.Verify(factory => factory.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockOutputPort.VerifyAll();
    }
    [Fact]
    public async Task Handle_ResetPasswordUsingGeneratedCode_InvalidPassword_ShouldFail()
    {
        // arrange

        // 1. We need to store the user data somehow
        var mockLogger = new Mock<ILogger<ResetPasswordUseCase>>();
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository
              .Setup(repo => repo.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.PasswordResponse("", true));

        // 2. The use case and star of this test
        var useCase = new ResetPasswordUseCase(mockLogger.Object, mockUserRepository.Object);

        // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
        // for final preparation to deliver back to the UI/web page/api response etc.
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act

        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        var response = await useCase.Handle(new ResetPasswordRequest(null, "me@email.com", string.Empty, "code"), mockOutputPort.Object);

        // assert
        Assert.False(response);
        mockUserRepository.Verify(factory => factory.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockOutputPort.VerifyAll();
    }
}