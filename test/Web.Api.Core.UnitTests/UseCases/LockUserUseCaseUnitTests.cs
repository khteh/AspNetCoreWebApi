using System;
using System.Threading.Tasks;
using Moq;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.UseCases;
using Xunit;
namespace Web.Api.Core.UnitTests.UseCases;

public class LockUserUseCaseUnitTests
{
    [Fact]
    public async Task Handle_ShouldThrow()
    {
        // arrange

        // 1. We need to store the user data somehow
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository
              .Setup(repo => repo.LockUser(It.IsAny<string>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.LockUserResponse("", true));

        // 2. The use case and star of this test
        var useCase = new LockUserUseCase(mockUserRepository.Object);

        // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
        // for final preparation to deliver back to the UI/web page/api response etc.
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act

        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        await Assert.ThrowsAsync<NotImplementedException>(async () => await useCase.Handle(new LockUserRequest("id"), mockOutputPort.Object));

        // assert
    }
    [Fact]
    public async Task Handle_LockUser_ShouldSucceed()
    {
        // arrange

        // 1. We need to store the user data somehow
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository
              .Setup(repo => repo.LockUser(It.IsAny<string>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.LockUserResponse("", true));

        // 2. The use case and star of this test
        var useCase = new LockUserUseCase(mockUserRepository.Object);

        // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
        // for final preparation to deliver back to the UI/web page/api response etc.
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act

        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        var response = await useCase.Lock("id", mockOutputPort.Object);

        // assert
        Assert.True(response);
        mockUserRepository.VerifyAll();
        mockOutputPort.VerifyAll();
    }
    [Fact]
    public async Task Handle_UnLockUser_ShouldSucceed()
    {
        // arrange

        // 1. We need to store the user data somehow
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository
              .Setup(repo => repo.UnLockUser(It.IsAny<string>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.LockUserResponse("", true));

        // 2. The use case and star of this test
        var useCase = new LockUserUseCase(mockUserRepository.Object);

        // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
        // for final preparation to deliver back to the UI/web page/api response etc.
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act

        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        var response = await useCase.UnLock("id", mockOutputPort.Object);

        // assert
        Assert.True(response);
        mockUserRepository.VerifyAll();
        mockOutputPort.VerifyAll();
    }
}