using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.UseCases;
using Web.Api.Infrastructure.Identity;
using Xunit;
namespace Web.Api.Infrastructure.UnitTests.UseCases;
public class TwoFactorRecoveryCodeSignInUseCaseUnitTests
{
    [Fact]
    public async void Handle_GivenValidRecoveryCode_ShouldSucceed()
    {
        // arrange
        AppUser appUser = new AppUser("", "", "", "");
        Mock<SignInManager<AppUser>> signInManager = new Mock<SignInManager<AppUser>>();
        signInManager.Setup(i => i.GetTwoFactorAuthenticationUserAsync()).ReturnsAsync(appUser);
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository.Setup(repo => repo.TwoFactorRecoveryCodeSignIn(It.IsAny<string>())).ReturnsAsync(new Core.DTO.GatewayResponses.Repositories.SignInResponse(Guid.Parse(appUser.Id), appUser.UserName, true));

        var useCase = new TwoFactorRecoveryCodeSignInUseCase(mockUserRepository.Object);
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act
        var response = await useCase.Handle(new TwoFactorRecoveryCodeSignInRequest("Code"), mockOutputPort.Object);

        // assert
        Assert.True(response);
        mockOutputPort.VerifyAll();
    }
    [Fact]
    public async void Handle_GivenInvalidCode_ShouldFail()
    {
        // arrange
        Mock<SignInManager<AppUser>> signInManager = new Mock<SignInManager<AppUser>>();
        signInManager.Setup(i => i.GetTwoFactorAuthenticationUserAsync()).ReturnsAsync((AppUser)null);

        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository.Setup(repo => repo.TwoFactorRecoveryCodeSignIn(It.IsAny<string>())).ReturnsAsync(new Core.DTO.GatewayResponses.Repositories.SignInResponse(Guid.Empty, string.Empty, false));

        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));
        var useCase = new TwoFactorRecoveryCodeSignInUseCase(mockUserRepository.Object);

        // act
        var response = await useCase.Handle(new TwoFactorRecoveryCodeSignInRequest("Code"), mockOutputPort.Object);

        // assert
        Assert.False(response);
        mockOutputPort.VerifyAll();
    }
}