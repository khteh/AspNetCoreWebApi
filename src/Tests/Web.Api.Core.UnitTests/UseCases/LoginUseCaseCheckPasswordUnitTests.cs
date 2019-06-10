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
using Web.Api.Core.DTO.GatewayResponses.Repositories;

namespace Web.Api.Core.UnitTests.UseCases
{
    public class LoginUseCaseCheckPasswordUnitTests
    {
        [Fact]
        public async void Handle_GivenValidCredentials_ShouldSucceed()
        {
            // arrange
            User user = new User("","","","");
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(repo => repo.FindUserByName(It.IsAny<string>())).ReturnsAsync(user);

            mockUserRepository.Setup(repo => repo.CheckPassword(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new LogInResponse(user, true));

            var mockJwtFactory = new Mock<IJwtFactory>();
            mockJwtFactory.Setup(factory => factory.GenerateEncodedToken(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new AccessToken("", 0));

            var mockTokenFactory = new Mock<ITokenFactory>();

            var useCase = new LoginUseCase(mockUserRepository.Object, mockJwtFactory.Object, mockTokenFactory.Object);

            var mockOutputPort = new Mock<IOutputPort<LoginResponse>>();
            mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<LoginResponse>()));

            // act
            var response = await useCase.Handle(new LoginRequest("userName", "password","127.0.0.1"), mockOutputPort.Object);

            // assert
            Assert.True(response);
            mockUserRepository.Verify(factory => factory.CheckPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockUserRepository.Verify(factory => factory.SignIn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
            mockOutputPort.VerifyAll();
            mockTokenFactory.VerifyAll();
            mockJwtFactory.VerifyAll();
        }

        [Fact]
        public async void Handle_GivenIncompleteCredentials_ShouldFail()
        {
            // arrange
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(repo => repo.FindUserByName(It.IsAny<string>())).ReturnsAsync(new User("","","",""));
            mockUserRepository.Setup(repo => repo.CheckPassword(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new LogInResponse(null, false));

            var mockJwtFactory = new Mock<IJwtFactory>();
            mockJwtFactory.Setup(factory => factory.GenerateEncodedToken(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new AccessToken("", 0));

            var mockTokenFactory = new Mock<ITokenFactory>();
            var useCase = new LoginUseCase(mockUserRepository.Object, mockJwtFactory.Object, mockTokenFactory.Object);

            var mockOutputPort = new Mock<IOutputPort<LoginResponse>>();
            mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<LoginResponse>()));

            // act
            var response = await useCase.Handle(new LoginRequest("", "password","127.0.0.1"), mockOutputPort.Object);

            // assert
            Assert.False(response);
            mockUserRepository.Verify(factory => factory.CheckPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mockUserRepository.Verify(factory => factory.SignIn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
            mockTokenFactory.Verify(factory => factory.GenerateToken(32), Times.Never);
            mockUserRepository.Verify(factory => factory.FindByName(""), Times.Never);
            mockOutputPort.VerifyAll();
            mockTokenFactory.VerifyAll();
            mockJwtFactory.Verify(factory => factory.GenerateEncodedToken(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        [Fact]
        public async void Handle_GivenUnknownCredentials_ShouldFail()
        {
            // arrange
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(repo => repo.FindUserByName(It.IsAny<string>())).ReturnsAsync((User)null);

            mockUserRepository.Setup(repo => repo.CheckPassword(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new LogInResponse(null, false));

            var mockJwtFactory = new Mock<IJwtFactory>();
            mockJwtFactory.Setup(factory => factory.GenerateEncodedToken(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new AccessToken("", 0));

            var mockTokenFactory = new Mock<ITokenFactory>();

            var useCase = new LoginUseCase(mockUserRepository.Object, mockJwtFactory.Object, mockTokenFactory.Object);

            var mockOutputPort = new Mock<IOutputPort<LoginResponse>>();
            mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<LoginResponse>()));

            // act
            var response = await useCase.Handle(new LoginRequest("", "password","127.0.0.1"), mockOutputPort.Object);

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
        public async void Handle_GivenInvalidPassword_ShouldFail()
        {
            // arrange
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(repo => repo.FindUserByName(It.IsAny<string>())).ReturnsAsync((User)null);

            mockUserRepository.Setup(repo => repo.CheckPassword(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new LogInResponse(null, false));

            var mockJwtFactory = new Mock<IJwtFactory>();
            mockJwtFactory.Setup(factory => factory.GenerateEncodedToken(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new AccessToken("", 0));

            var mockTokenFactory = new Mock<ITokenFactory>();

            var useCase = new LoginUseCase(mockUserRepository.Object, mockJwtFactory.Object, mockTokenFactory.Object);

            var mockOutputPort = new Mock<IOutputPort<LoginResponse>>();
            mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<LoginResponse>()));

            // act
            var response = await useCase.Handle(new LoginRequest("", "password","127.0.0.1"), mockOutputPort.Object);

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
}