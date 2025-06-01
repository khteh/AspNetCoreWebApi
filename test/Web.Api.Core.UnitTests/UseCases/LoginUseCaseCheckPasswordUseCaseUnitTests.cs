using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.Services;
using Web.Api.Core.UseCases;
using Xunit;

namespace Web.Api.Core.UnitTests.UseCases;

public class LogInUseCaseCheckPasswordUnitTests
{
    [Fact]
    public async Task Handle_GivenValidCredentials_ShouldSucceed()
    {
        // arrange
        var user = new User("", "", "", "", "", "");
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository.Setup(repo => repo.FindUserByName(It.IsAny<string>())).ReturnsAsync(user);
        mockUserRepository.Setup(repo => repo.CheckPassword(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Web.Api.Core.DTO.GatewayResponses.Repositories.LogInResponse(user, true));

        var mockJwtFactory = new Mock<IJwtFactory>();
        mockJwtFactory.Setup(factory => factory.GenerateEncodedToken(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new AccessToken("", 0));

        var mockTokenFactory = new Mock<ITokenFactory>();
        var mockLogger = new Mock<ILogger<LogInUseCase>>();
        var useCase = new LogInUseCase(mockLogger.Object, mockUserRepository.Object, mockJwtFactory.Object, mockTokenFactory.Object);
        var mockOutputPort = new Mock<IOutputPort<LogInResponse>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<LogInResponse>()));

        // act
        var response = await useCase.Handle(new LogInRequest("userName", "password", "127.0.0.1"), mockOutputPort.Object);

        // assert
        Assert.True(response);
        mockUserRepository.Verify(factory => factory.CheckPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        mockUserRepository.Verify(factory => factory.SignIn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
        mockOutputPort.VerifyAll();
        mockTokenFactory.VerifyAll();
        mockJwtFactory.VerifyAll();
    }
    [Fact]
    public async Task Handle_GivenIncompleteCredentials_ShouldFail()
    {
        // arrange
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository.Setup(repo => repo.FindUserByName(It.IsAny<string>())).ReturnsAsync(new User("", "", "", "", "", ""));
        mockUserRepository.Setup(repo => repo.CheckPassword(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Web.Api.Core.DTO.GatewayResponses.Repositories.LogInResponse(null, false));

        var mockJwtFactory = new Mock<IJwtFactory>();
        mockJwtFactory.Setup(factory => factory.GenerateEncodedToken(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new AccessToken("", 0));

        var mockTokenFactory = new Mock<ITokenFactory>();
        var mockLogger = new Mock<ILogger<LogInUseCase>>();
        var useCase = new LogInUseCase(mockLogger.Object, mockUserRepository.Object, mockJwtFactory.Object, mockTokenFactory.Object);

        var mockOutputPort = new Mock<IOutputPort<LogInResponse>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<LogInResponse>()));

        // act
        var response = await useCase.Handle(new LogInRequest("", "password", "127.0.0.1"), mockOutputPort.Object);

        // assert
        Assert.False(response);
        mockUserRepository.Verify(factory => factory.CheckPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockUserRepository.Verify(factory => factory.SignIn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
        mockTokenFactory.Verify(factory => factory.GenerateToken(32), Times.Never);
        mockUserRepository.Verify(factory => factory.FindByName(""), Times.Never);
        mockOutputPort.VerifyAll();
        mockTokenFactory.VerifyAll();
        mockJwtFactory.Verify(factory => factory.GenerateEncodedToken(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
    [Fact]
    public async Task Handle_GivenUnknownCredentials_ShouldFail()
    {
        // arrange
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository.Setup(repo => repo.FindUserByName(It.IsAny<string>())).ReturnsAsync((User)null);
        mockUserRepository.Setup(repo => repo.CheckPassword(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Web.Api.Core.DTO.GatewayResponses.Repositories.LogInResponse(null, false));

        var mockJwtFactory = new Mock<IJwtFactory>();
        mockJwtFactory.Setup(factory => factory.GenerateEncodedToken(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new AccessToken("", 0));

        var mockTokenFactory = new Mock<ITokenFactory>();
        var mockLogger = new Mock<ILogger<LogInUseCase>>();
        var useCase = new LogInUseCase(mockLogger.Object, mockUserRepository.Object, mockJwtFactory.Object, mockTokenFactory.Object);
        var mockOutputPort = new Mock<IOutputPort<LogInResponse>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<LogInResponse>()));

        // act
        var response = await useCase.Handle(new LogInRequest("", "password", "127.0.0.1"), mockOutputPort.Object);

        // assert
        Assert.False(response);
        mockTokenFactory.Verify(factory => factory.GenerateToken(32), Times.Never);
        mockUserRepository.Verify(factory => factory.FindByName(""), Times.Never);
        mockUserRepository.Verify(factory => factory.CheckPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockOutputPort.VerifyAll();
        mockTokenFactory.VerifyAll();
        mockJwtFactory.Verify(factory => factory.GenerateEncodedToken(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
    [Fact]
    public async Task Handle_GivenInvalidPassword_ShouldFail()
    {
        // arrange
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository.Setup(repo => repo.FindUserByName(It.IsAny<string>())).ReturnsAsync((User)null);
        mockUserRepository.Setup(repo => repo.CheckPassword(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Web.Api.Core.DTO.GatewayResponses.Repositories.LogInResponse(null, false));

        var mockJwtFactory = new Mock<IJwtFactory>();
        mockJwtFactory.Setup(factory => factory.GenerateEncodedToken(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new AccessToken("", 0));

        var mockTokenFactory = new Mock<ITokenFactory>();
        var mockLogger = new Mock<ILogger<LogInUseCase>>();
        var useCase = new LogInUseCase(mockLogger.Object, mockUserRepository.Object, mockJwtFactory.Object, mockTokenFactory.Object);
        var mockOutputPort = new Mock<IOutputPort<LogInResponse>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<LogInResponse>()));

        // act
        var response = await useCase.Handle(new LogInRequest("", "password", "127.0.0.1"), mockOutputPort.Object);

        // assert
        Assert.False(response);
        mockTokenFactory.Verify(factory => factory.GenerateToken(32), Times.Never);
        mockUserRepository.Verify(factory => factory.FindByName(""), Times.Never);
        mockUserRepository.Verify(factory => factory.CheckPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockOutputPort.VerifyAll();
        mockTokenFactory.VerifyAll();
        mockJwtFactory.Verify(factory => factory.GenerateEncodedToken(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}