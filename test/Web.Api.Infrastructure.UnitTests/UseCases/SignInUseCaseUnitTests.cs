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
public class SignInUseCaseUnitTests
{
    [Fact]
    public async void Handle_GivenValidCredentials_ShouldSucceed()
    {
        // arrange
        AppUser appUser = new AppUser("", "", "", "");
        List<Claim> claims = new List<Claim>();
        Mock<UserManager<AppUser>> userManager = new Mock<UserManager<AppUser>>();
        userManager.Setup(i => i.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(appUser);
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository.Setup(repo => repo.SignIn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(new Core.DTO.GatewayResponses.Repositories.SignInResponse(Guid.Parse(appUser.Id), appUser.UserName, true));

        var useCase = new SignInUseCase(mockUserRepository.Object);
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act
        var response = await useCase.Handle(new SignInRequest("userName", "password", "whatever", true, true, false), mockOutputPort.Object);

        // assert
        Assert.True(response);
        mockUserRepository.Verify(factory => factory.CheckPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockUserRepository.Verify(factory => factory.SignIn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
        mockOutputPort.VerifyAll();
    }
    [Fact]
    public async void Handle_GivenIncompleteCredentials_ShouldFail()
    {
        // arrange
        Mock<UserManager<AppUser>> userManager = new Mock<UserManager<AppUser>>();
        userManager.Setup(i => i.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new AppUser("", "", "", ""));

        List<Claim> claims = new List<Claim>();
        var mockUserRepository = new Mock<IUserRepository>();

        mockUserRepository.Setup(repo => repo.SignIn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(new Core.DTO.GatewayResponses.Repositories.SignInResponse(Guid.Empty, string.Empty, false));

        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));
        var useCase = new SignInUseCase(mockUserRepository.Object);

        // act
        var response = await useCase.Handle(new SignInRequest("", "password", "whatever", true, true, false), mockOutputPort.Object);


        // assert
        Assert.False(response);
        mockUserRepository.Verify(factory => factory.CheckPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockUserRepository.Verify(factory => factory.SignIn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
        mockUserRepository.Verify(factory => factory.FindByName(""), Times.Never);
        mockOutputPort.VerifyAll();
    }
    [Fact]
    public async void Handle_GivenUnknownCredentials_ShouldFail()
    {
        // arrange
        Mock<UserManager<AppUser>> userManager = new Mock<UserManager<AppUser>>();
        userManager.Setup(i => i.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((AppUser)null);

        List<Claim> claims = new List<Claim>();
        var mockUserRepository = new Mock<IUserRepository>();

        mockUserRepository.Setup(repo => repo.SignIn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(new Core.DTO.GatewayResponses.Repositories.SignInResponse(Guid.Empty, string.Empty, false));

        var useCase = new SignInUseCase(mockUserRepository.Object);

        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act
        var response = await useCase.Handle(new SignInRequest("", "password", "whatever", true, true, false), mockOutputPort.Object);

        // assert
        Assert.False(response);
        mockUserRepository.Verify(factory => factory.FindByName(""), Times.Never);
        mockUserRepository.Verify(factory => factory.CheckPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockOutputPort.VerifyAll();
    }
    [Fact]
    public async void Handle_GivenInvalidPassword_ShouldFail()
    {
        // arrange
        Mock<UserManager<AppUser>> userManager = new Mock<UserManager<AppUser>>();
        userManager.Setup(i => i.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((AppUser)null);

        List<Claim> claims = new List<Claim>();
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository.Setup(repo => repo.SignIn(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(new Core.DTO.GatewayResponses.Repositories.SignInResponse(Guid.Empty, string.Empty, false));

        var useCase = new SignInUseCase(mockUserRepository.Object);
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act
        var response = await useCase.Handle(new SignInRequest("", "password", "whatever", true, true, false), mockOutputPort.Object);

        // assert
        Assert.False(response);
        mockUserRepository.Verify(factory => factory.FindByName(""), Times.Never);
        mockUserRepository.Verify(factory => factory.CheckPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockOutputPort.VerifyAll();
    }
}