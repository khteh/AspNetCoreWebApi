using Moq;
using Web.Api.Core.DTO.GatewayResponses.Repositories;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.UseCases;
using Xunit;

namespace Web.Api.Core.UnitTests.UseCases
{
    public class FindUserUseCaseUnitTests
    {
        [Fact]
        public async void Handle_FindById_Succeed()
        {
            // arrange

            // 1. We need to store the user data somehow
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository
              .Setup(repo => repo.FindById(It.IsAny<string>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.FindUserResponse("", true));

            // 2. The use case and star of this test
            var useCase = new FindUserUseCase(mockUserRepository.Object);

            // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
            // for final preparation to deliver back to the UI/web page/api response etc.
            var mockOutputPort = new Mock<IOutputPort<DTO.UseCaseResponses.FindUserResponse>>();
            mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<DTO.UseCaseResponses.FindUserResponse>()));

            // act

            // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
            var response = await useCase.Handle(new FindUserRequest(string.Empty, string.Empty, "id"), mockOutputPort.Object);

            // assert
            Assert.True(response);
            mockUserRepository.VerifyAll();
            mockOutputPort.VerifyAll();
        }
        [Fact]
        public async void Handle_FindByUsername_Succeed()
        {
            // arrange

            // 1. We need to store the user data somehow
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository
              .Setup(repo => repo.FindByName(It.IsAny<string>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.FindUserResponse("", true));

            // 2. The use case and star of this test
            var useCase = new FindUserUseCase(mockUserRepository.Object);

            // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
            // for final preparation to deliver back to the UI/web page/api response etc.
            var mockOutputPort = new Mock<IOutputPort<DTO.UseCaseResponses.FindUserResponse>>();
            mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<DTO.UseCaseResponses.FindUserResponse>()));

            // act

            // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
            var response = await useCase.Handle(new FindUserRequest(string.Empty, "username", string.Empty), mockOutputPort.Object);

            // assert
            Assert.True(response);
            mockUserRepository.VerifyAll();
            mockOutputPort.VerifyAll();
        }
        [Fact]
        public async void Handle_FindByEmail_Succeed()
        {
            // arrange

            // 1. We need to store the user data somehow
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository
              .Setup(repo => repo.FindByEmail(It.IsAny<string>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.FindUserResponse("", true));

            // 2. The use case and star of this test
            var useCase = new FindUserUseCase(mockUserRepository.Object);

            // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
            // for final preparation to deliver back to the UI/web page/api response etc.
            var mockOutputPort = new Mock<IOutputPort<DTO.UseCaseResponses.FindUserResponse>>();
            mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<DTO.UseCaseResponses.FindUserResponse>()));

            // act

            // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
            var response = await useCase.Handle(new FindUserRequest("email", string.Empty, string.Empty), mockOutputPort.Object);

            // assert
            Assert.True(response);
            mockUserRepository.VerifyAll();
            mockOutputPort.VerifyAll();
        }
        [Fact]
        public async void Handle_FindByMultipleParams_ShouldPassWithFindById()
        {
            // arrange

            // 1. We need to store the user data somehow
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository
              .Setup(repo => repo.FindById(It.IsAny<string>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.FindUserResponse("", true));

            // 2. The use case and star of this test
            var useCase = new FindUserUseCase(mockUserRepository.Object);

            // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
            // for final preparation to deliver back to the UI/web page/api response etc.
            var mockOutputPort = new Mock<IOutputPort<DTO.UseCaseResponses.FindUserResponse>>();
            mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<DTO.UseCaseResponses.FindUserResponse>()));

            // act

            // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
            var response = await useCase.Handle(new FindUserRequest("email", "username", "id"), mockOutputPort.Object);

            // assert
            Assert.True(response);
            mockUserRepository.VerifyAll();
            mockOutputPort.VerifyAll();
        }
        [Fact]
        public async void Handle_FindByInvalidEmptyParams_ShouldFail()
        {
            // arrange

            // 1. We need to store the user data somehow
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository
              .Setup(repo => repo.FindById(It.IsAny<string>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.FindUserResponse("", true));

            // 2. The use case and star of this test
            var useCase = new FindUserUseCase(mockUserRepository.Object);

            // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
            // for final preparation to deliver back to the UI/web page/api response etc.
            var mockOutputPort = new Mock<IOutputPort<DTO.UseCaseResponses.FindUserResponse>>();
            mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<DTO.UseCaseResponses.FindUserResponse>()));

            // act

            // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
            var response = await useCase.Handle(new FindUserRequest(string.Empty, string.Empty, string.Empty), mockOutputPort.Object);

            // assert
            Assert.False(response);
            mockUserRepository.Verify(factory => factory.FindById(string.Empty), Times.Never);
            mockOutputPort.VerifyAll();
        }
    }
}