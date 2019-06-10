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
    public class DeleteUserUseCaseUnitTests
    {
        [Fact]
        public async void Handle_GivenValidRegistrationDetails_ShouldSucceed()
        {
            // arrange

            // 1. We need to store the user data somehow
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository
              .Setup(repo => repo.Delete(It.IsAny<string>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.DeleteUserResponse("", true));

            // 2. The use case and star of this test
            var useCase = new DeleteUserUseCase(mockUserRepository.Object);

            // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
            // for final preparation to deliver back to the UI/web page/api response etc.
            var mockOutputPort = new Mock<IOutputPort<DTO.UseCaseResponses.DeleteUserResponse>>();
            mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<DTO.UseCaseResponses.DeleteUserResponse>()));

            // act

            // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
            var response = await useCase.Handle(new DeleteUserRequest("userName"), mockOutputPort.Object);

            // assert
            Assert.True(response);
            mockUserRepository.VerifyAll();
            mockOutputPort.VerifyAll();
        }
    }
}