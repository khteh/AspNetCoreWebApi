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
    public class ChanagePasswordUseCaseUnitTests
    {
        [Fact]
        public async void Handle_ChangePassword_ShouldSucceed()
        {
            // arrange

            // 1. We need to store the user data somehow
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository
              .Setup(repo => repo.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.PasswordResponse("", true));

            // 2. The use case and star of this test
            var useCase = new ChangePasswordUseCase(mockUserRepository.Object);

            // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
            // for final preparation to deliver back to the UI/web page/api response etc.
            var mockOutputPort = new Mock<IOutputPort<ChangePasswordResponse>>();
            mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<ChangePasswordResponse>()));

            // act

            // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
            var response = await useCase.Handle(new ChangePasswordRequest("firstName", "oldPassword", "newPassword"), mockOutputPort.Object);

            // assert
            Assert.True(response);            
            mockUserRepository.VerifyAll();
            mockOutputPort.VerifyAll();
        }
    }
}