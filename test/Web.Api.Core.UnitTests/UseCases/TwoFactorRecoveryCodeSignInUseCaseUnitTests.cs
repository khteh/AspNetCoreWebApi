using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Moq;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.UseCases;
using Xunit;
namespace Web.Api.Core.UnitTests.UseCases;
public class TwoFactorRecoveryCodeSignInUseCaseUnitTests
{
    [Fact]
    public async void Handle_TwoFactorRecoveryCodeSignIn_WithInvalidCode_ShouldFail()
    {
        // arrange

        // 1. We need to store the user data somehow
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository
              .Setup(repo => repo.TwoFactorRecoveryCodeSignIn(It.IsAny<string>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.SignInResponse(Guid.Empty, string.Empty, false));

        // 2. The use case and star of this test
        var useCase = new TwoFactorRecoveryCodeSignInUseCase(mockUserRepository.Object);

        // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
        // for final preparation to deliver back to the UI/web page/api response etc.
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act

        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        // public SignInRequest(string username, string password, string remoteIP, bool isPersistent = false, bool lockoutOnFailure = true, bool isMobile = false)
        var response = await useCase.Handle(new TwoFactorRecoveryCodeSignInRequest(string.Empty), mockOutputPort.Object);

        // assert
        Assert.False(response);
        mockUserRepository.Verify(factory => factory.TwoFactorRecoveryCodeSignIn(It.IsAny<string>()), Times.Never);
        mockOutputPort.VerifyAll();
    }
    [Fact]
    public async void Handle_TwoFactorRecoveryCodeSignIn_WithSignInManagerError_ShouldFail()
    {
        // arrange

        // 1. We need to store the user data somehow
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository
              .Setup(repo => repo.TwoFactorRecoveryCodeSignIn(It.IsAny<string>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.SignInResponse(Guid.Empty, string.Empty, false));

        // 2. The use case and star of this test
        var useCase = new TwoFactorRecoveryCodeSignInUseCase(mockUserRepository.Object);

        // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
        // for final preparation to deliver back to the UI/web page/api response etc.
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act

        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        // public SignInRequest(string username, string password, string remoteIP, bool isPersistent = false, bool lockoutOnFailure = true, bool isMobile = false)
        var response = await useCase.Handle(new TwoFactorRecoveryCodeSignInRequest("Code"), mockOutputPort.Object);

        // assert
        Assert.False(response);
        mockOutputPort.VerifyAll();
    }
    [Fact]
    public async void Handle_TwoFactorRecoveryCodeSignIn_ShouldSucceed()
    {
        // arrange

        // 1. We need to store the user data somehow
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository
              .Setup(repo => repo.TwoFactorRecoveryCodeSignIn(It.IsAny<string>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.SignInResponse(Guid.NewGuid(), string.Empty, true));

        // 2. The use case and star of this test
        var useCase = new TwoFactorRecoveryCodeSignInUseCase(mockUserRepository.Object);

        // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
        // for final preparation to deliver back to the UI/web page/api response etc.
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act

        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        // public SignInRequest(string username, string password, string remoteIP, bool isPersistent = false, bool lockoutOnFailure = true, bool isMobile = false)
        var response = await useCase.Handle(new TwoFactorRecoveryCodeSignInRequest("Code"), mockOutputPort.Object);

        // assert
        Assert.True(response);
        mockUserRepository.VerifyAll();
        // public async Task<SignInResponse> SignIn(string username, string password, string remoteIP, bool rememberMe, bool logoutOnFailure)
        mockOutputPort.VerifyAll();
    }
}