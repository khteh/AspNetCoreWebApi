using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Claims;
using System.Security.Permissions;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.UseCases;
using Web.Api.Infrastructure.Identity;
using Xunit;
namespace Web.Api.Infrastructure.UnitTests.UseCases;
public class ConfirmEmailChangeUseCaseUnitTests
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
        userManager.Setup(i => i.SetUserNameAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository.Setup(repo => repo.ConfirmEmailChange(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Core.DTO.GatewayResponses.Repositories.FindUserResponse(appUser.Id.ToString(), user, true));

        var useCase = new ConfirmEmailChangeUseCase(mockUserRepository.Object);
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act
        var response = await useCase.Handle(new ConfirmEmailChangeRequest("id", "me@email.com", "code"), mockOutputPort.Object);

        // assert
        Assert.True(response);
        mockUserRepository.VerifyAll();
        mockOutputPort.VerifyAll();
    }
    [Fact]
    public async void Handle_GivenInValidEmail_ShouldFail()
    {
        // arrange
        AppUser appUser = new AppUser("", "", "", "");
        User user = new User();
        List<Claim> claims = new List<Claim>();
        Mock<UserManager<AppUser>> userManager = new Mock<UserManager<AppUser>>();
        userManager.Setup(i => i.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(appUser);
        userManager.Setup(i => i.SetUserNameAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(new IdentityError[] { new IdentityError() { Code = "123", Description = "Fail" } }));
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository.Setup(repo => repo.ConfirmEmailChange(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Core.DTO.GatewayResponses.Repositories.FindUserResponse(appUser.Id.ToString(), user, false));

        var useCase = new ConfirmEmailChangeUseCase(mockUserRepository.Object);
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act
        var response = await useCase.Handle(new ConfirmEmailChangeRequest("id", "email", "code"), mockOutputPort.Object);

        // assert
        Assert.False(response);
        mockUserRepository.Verify(factory => factory.ConfirmEmailChange(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockOutputPort.VerifyAll();
    }
    [Fact]
    public async void Handle_GivenInValidCode_ShouldFail()
    {
        // arrange
        AppUser appUser = new AppUser("", "", "", "");
        User user = new User();
        List<Claim> claims = new List<Claim>();
        Mock<UserManager<AppUser>> userManager = new Mock<UserManager<AppUser>>();
        userManager.Setup(i => i.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(appUser);
        userManager.Setup(i => i.SetUserNameAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(new IdentityError[] { new IdentityError() { Code = "123", Description = "Fail" } }));
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository.Setup(repo => repo.ConfirmEmailChange(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Core.DTO.GatewayResponses.Repositories.FindUserResponse(appUser.Id.ToString(), user, false));

        var useCase = new ConfirmEmailChangeUseCase(mockUserRepository.Object);
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act
        var response = await useCase.Handle(new ConfirmEmailChangeRequest("id", "me@email,com", string.Empty), mockOutputPort.Object);

        // assert
        Assert.False(response);
        mockUserRepository.Verify(factory => factory.ConfirmEmailChange(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockOutputPort.VerifyAll();
    }
}