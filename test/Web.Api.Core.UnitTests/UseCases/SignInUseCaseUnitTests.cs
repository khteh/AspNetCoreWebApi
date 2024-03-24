using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Moq;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.UseCases;
using Xunit;
namespace Web.Api.Core.UnitTests.UseCases;
public class SignInUseCaseUnitTests
{
    [Fact]
    public async void Handle_SignIn_Mobile_InvalidCredentials_ShouldFail()
    {
        // arrange

        // 1. We need to store the user data somehow
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository
              .Setup(repo => repo.SignInMobile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.SignInResponse(Guid.Empty, string.Empty, true));

        // 2. The use case and star of this test
        var useCase = new SignInUseCase(mockUserRepository.Object);

        // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
        // for final preparation to deliver back to the UI/web page/api response etc.
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act

        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        // public SignInRequest(string username, string password, string remoteIP, bool isPersistent = false, bool lockoutOnFailure = true, bool isMobile = false)
        var response = await useCase.Handle(new SignInRequest(string.Empty, string.Empty, "remoteIP", true, true, true), mockOutputPort.Object);

        // assert
        Assert.False(response);
        mockUserRepository.Verify(factory => factory.SignInMobile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        // public async Task<SignInResponse> SignIn(string username, string password, string remoteIP, bool rememberMe, bool logoutOnFailure)
        mockUserRepository.Verify(factory => factory.SignIn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
        mockOutputPort.VerifyAll();
    }
    [Fact]
    public async void Handle_SignIn_Mobile_ShouldSucceed()
    {
        // arrange

        // 1. We need to store the user data somehow
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository
              .Setup(repo => repo.SignInMobile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.SignInResponse(Guid.NewGuid(), string.Empty, true));

        // 2. The use case and star of this test
        var useCase = new SignInUseCase(mockUserRepository.Object);

        // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
        // for final preparation to deliver back to the UI/web page/api response etc.
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act

        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        // public SignInRequest(string username, string password, string remoteIP, bool isPersistent = false, bool lockoutOnFailure = true, bool isMobile = false)
        var response = await useCase.Handle(new SignInRequest("UserName", "password", "remoteIP", true, true, true), mockOutputPort.Object);

        // assert
        Assert.True(response);
        mockUserRepository.VerifyAll();
        // public async Task<SignInResponse> SignIn(string username, string password, string remoteIP, bool rememberMe, bool logoutOnFailure)
        mockUserRepository.Verify(factory => factory.SignIn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
        mockOutputPort.VerifyAll();
    }
    [Fact]
    public async void Handle_SignIn_InvalidCredentials_ShouldFail()
    {
        // arrange

        // 1. We need to store the user data somehow
        var mockUserRepository = new Mock<IUserRepository>();
        // public async Task<SignInResponse> SignIn(string username, string password, string remoteIP, bool rememberMe, bool logoutOnFailure)
        mockUserRepository
              .Setup(repo => repo.SignIn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.SignInResponse(Guid.Empty, string.Empty, true));

        // 2. The use case and star of this test
        var useCase = new SignInUseCase(mockUserRepository.Object);

        // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
        // for final preparation to deliver back to the UI/web page/api response etc.
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act

        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        // public SignInRequest(string username, string password, string remoteIP, bool isPersistent = false, bool lockoutOnFailure = true, bool isMobile = false)
        var response = await useCase.Handle(new SignInRequest(string.Empty, string.Empty, "remoteIP", true, true, false), mockOutputPort.Object);

        // assert
        Assert.False(response);
        mockUserRepository.Verify(factory => factory.SignInMobile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        // public async Task<SignInResponse> SignIn(string username, string password, string remoteIP, bool rememberMe, bool logoutOnFailure)
        mockUserRepository.Verify(factory => factory.SignInMobile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        mockOutputPort.VerifyAll();
    }
    [Fact]
    public async void Handle_SignIn_ShouldSucceed()
    {
        // arrange

        // 1. We need to store the user data somehow
        var mockUserRepository = new Mock<IUserRepository>();
        // public async Task<SignInResponse> SignIn(string username, string password, string remoteIP, bool rememberMe, bool logoutOnFailure)
        mockUserRepository
              .Setup(repo => repo.SignIn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.SignInResponse(Guid.NewGuid(), string.Empty, true));

        // 2. The use case and star of this test
        var useCase = new SignInUseCase(mockUserRepository.Object);

        // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
        // for final preparation to deliver back to the UI/web page/api response etc.
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act

        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        // public SignInRequest(string username, string password, string remoteIP, bool isPersistent = false, bool lockoutOnFailure = true, bool isMobile = false)
        var response = await useCase.Handle(new SignInRequest("UserName", "password", "remoteIP", true, true, false), mockOutputPort.Object);

        // assert
        Assert.True(response);
        mockUserRepository.VerifyAll();
        // public async Task<SignInResponse> SignIn(string username, string password, string remoteIP, bool rememberMe, bool logoutOnFailure)
        mockUserRepository.Verify(factory => factory.SignInMobile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        mockOutputPort.VerifyAll();
    }
    [Fact]
    public async void Handle_SignIn_WithClaims_WithInvalidID_ShouldFail()
    {
        // arrange

        // 1. We need to store the user data somehow
        var mockUserRepository = new Mock<IUserRepository>();
        // public async Task<SignInResponse> SignInWithClaims(string identityId, List<Claim> claims, AuthenticationProperties authProperties)
        mockUserRepository
              .Setup(repo => repo.SignInWithClaims(It.IsAny<string>(), It.IsAny<List<Claim>>(), It.IsAny<AuthenticationProperties>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.SignInResponse(Guid.Empty, string.Empty, true));

        // 2. The use case and star of this test
        var useCase = new SignInWithClaimsUseCase(mockUserRepository.Object);

        // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
        // for final preparation to deliver back to the UI/web page/api response etc.
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act

        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        // public SignInWithClaimsRequest(string identityId, List<Claim> claims, AuthenticationProperties authProperties)
        var response = await useCase.Handle(new SignInWithClaimsRequest(string.Empty, new List<Claim>() { }, new AuthenticationProperties()), mockOutputPort.Object);

        // assert
        Assert.False(response);
        mockUserRepository.Verify(factory => factory.SignInWithClaims(It.IsAny<string>(), It.IsAny<List<Claim>>(), It.IsAny<AuthenticationProperties>()), Times.Never);
        // public async Task<SignInResponse> SignIn(string username, string password, string remoteIP, bool rememberMe, bool logoutOnFailure)
        mockOutputPort.VerifyAll();
    }
    [Fact]
    public async void Handle_SignIn_WithClaims_WithValidID_ShouldSucceed()
    {
        // arrange

        // 1. We need to store the user data somehow
        var mockUserRepository = new Mock<IUserRepository>();
        // public async Task<SignInResponse> SignInWithClaims(string identityId, List<Claim> claims, AuthenticationProperties authProperties)
        mockUserRepository
              .Setup(repo => repo.SignInWithClaims(It.IsAny<string>(), It.IsAny<List<Claim>>(), It.IsAny<AuthenticationProperties>()))
              .ReturnsAsync(new DTO.GatewayResponses.Repositories.SignInResponse(Guid.Empty, string.Empty, true));

        // 2. The use case and star of this test
        var useCase = new SignInWithClaimsUseCase(mockUserRepository.Object);

        // 3. The output port is the mechanism to pass response data from the use case to a Presenter 
        // for final preparation to deliver back to the UI/web page/api response etc.
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act

        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        // public SignInWithClaimsRequest(string identityId, List<Claim> claims, AuthenticationProperties authProperties)
        var response = await useCase.Handle(new SignInWithClaimsRequest(Guid.NewGuid().ToString(), new List<Claim>() { }, new AuthenticationProperties()), mockOutputPort.Object);

        // assert
        Assert.False(response);
        mockUserRepository.VerifyAll();
        // public async Task<SignInResponse> SignIn(string username, string password, string remoteIP, bool rememberMe, bool logoutOnFailure)
        mockOutputPort.VerifyAll();
    }
}