using Castle.Core.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.UseCases;
using Web.Api.Core.UseCases;
using Web.Api.Infrastructure.Identity;
using Xunit;
namespace Web.Api.Infrastructure.UnitTests.UseCases;
public class GenerateChangeEmailTokenUseCaseUnitTests
{
    [Fact]
    public async void Handle_GivenValidCredentials_ShouldSucceed()
    {
        // arrange
        AppUser appUser = new AppUser("", "", "", "");
        User user = new User();
        List<Claim> claims = new List<Claim>();
        Mock<UserManager<AppUser>> userManager = new Mock<UserManager<AppUser>>();
        userManager.Setup(i => i.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(appUser);
        userManager.Setup(i => i.GenerateChangeEmailTokenAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync("GenerateChangeEmailToken_Code");
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository.Setup(repo => repo.GenerateChangeEmailToken(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Core.DTO.GatewayResponses.Repositories.CodeResponse(appUser.Id.ToString(), "GenerateChangeEmailToken_Code", true));
        var mockLogger = new Mock<ILogger<GenerateChangeEmailTokenUseCase>>();
        var useCase = new GenerateChangeEmailTokenUseCase(mockUserRepository.Object);
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act
        var response = await useCase.Handle(new GenerateChangeEmailTokenRequest("id", "me@email.com"), mockOutputPort.Object);

        // assert
        Assert.True(response);
        mockUserRepository.Verify(factory => factory.CheckPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockOutputPort.VerifyAll();
    }
    [Fact]
    public async void Handle_GivenValidCredentials_InvalidEmail_ShoulFail()
    {
        // arrange
        AppUser appUser = new AppUser("", "", "", "");
        User user = new User();
        List<Claim> claims = new List<Claim>();
        Mock<UserManager<AppUser>> userManager = new Mock<UserManager<AppUser>>();
        userManager.Setup(i => i.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(appUser);
        userManager.Setup(i => i.GenerateChangeEmailTokenAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync("GenerateChangeEmailToken_Code");
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository.Setup(repo => repo.GenerateChangeEmailToken(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Core.DTO.GatewayResponses.Repositories.CodeResponse(appUser.Id.ToString(), "GenerateChangeEmailToken_Code", true));
        var mockLogger = new Mock<ILogger<GenerateChangeEmailTokenUseCase>>();
        var useCase = new GenerateChangeEmailTokenUseCase(mockUserRepository.Object);
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act
        var response = await useCase.Handle(new GenerateChangeEmailTokenRequest("id", "email"), mockOutputPort.Object);

        // assert
        Assert.False(response);
        mockUserRepository.Verify(factory => factory.CheckPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockOutputPort.VerifyAll();
    }
    [Fact]
    public async void Handle_GivenINValidCredentials_ShouldFail()
    {
        // arrange
        AppUser appUser = new AppUser("", "", "", "");
        User user = new User();
        List<Claim> claims = new List<Claim>();
        Mock<UserManager<AppUser>> userManager = new Mock<UserManager<AppUser>>();
        userManager.Setup(i => i.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(appUser);
        userManager.Setup(i => i.GenerateChangeEmailTokenAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(string.Empty);
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository.Setup(repo => repo.GenerateChangeEmailToken(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Core.DTO.GatewayResponses.Repositories.CodeResponse(appUser.Id.ToString(), string.Empty, false));

        var mockLogger = new Mock<ILogger<GenerateChangeEmailTokenUseCase>>();
        var useCase = new GenerateChangeEmailTokenUseCase(mockUserRepository.Object);
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act
        var response = await useCase.Handle(new GenerateChangeEmailTokenRequest("id", "code"), mockOutputPort.Object);

        // assert
        Assert.False(response);
        mockOutputPort.VerifyAll();
    }
}