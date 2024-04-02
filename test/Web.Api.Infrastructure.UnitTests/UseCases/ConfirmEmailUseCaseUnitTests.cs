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
public class ConfirmEmailUseCaseUnitTests
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
        mockUserRepository.Setup(repo => repo.ConfirmEmail(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Core.DTO.GatewayResponses.Repositories.FindUserResponse(appUser.Id.ToString(), user, true));
        var mockLogger = new Mock<ILogger<ConfirmEmailUseCase>>();
        var useCase = new ConfirmEmailUseCase(mockLogger.Object, mockUserRepository.Object);
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act
        var response = await useCase.Handle(new ConfirmEmailRequest("id", "code"), mockOutputPort.Object);

        // assert
        Assert.True(response);
        mockUserRepository.VerifyAll();
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
        mockUserRepository.Setup(repo => repo.ConfirmEmail(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Core.DTO.GatewayResponses.Repositories.FindUserResponse(appUser.Id.ToString(), user, false));

        var mockLogger = new Mock<ILogger<ConfirmEmailUseCase>>();
        var useCase = new ConfirmEmailUseCase(mockLogger.Object, mockUserRepository.Object);
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act
        var response = await useCase.Handle(new ConfirmEmailRequest("id", string.Empty), mockOutputPort.Object);

        // assert
        Assert.False(response);
        mockUserRepository.Verify(factory => factory.ConfirmEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockOutputPort.VerifyAll();
    }
}