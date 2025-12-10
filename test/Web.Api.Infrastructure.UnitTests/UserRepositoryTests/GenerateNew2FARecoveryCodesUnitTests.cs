using Castle.Core.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.UseCases;
using Web.Api.Core.UseCases;
using Web.Api.Infrastructure.Identity;
using Xunit;
namespace Web.Api.Infrastructure.UnitTests.UserRepository;

public class GenerateNew2FARecoveryCodesUseCaseUnitTests
{
    [Fact]
    public async Task Handle_GivenValidCredentials_ShouldSucceed()
    {
        // arrange
        AppUser appUser = new AppUser("", "", "", "");
        User user = new User();
        List<Claim> claims = new List<Claim>();
        Mock<UserManager<AppUser>> userManager = new Mock<UserManager<AppUser>>();
        userManager.Setup(i => i.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(appUser);
        userManager.Setup(i => i.GenerateNewTwoFactorRecoveryCodesAsync(It.IsAny<AppUser>(), It.IsAny<int>())).ReturnsAsync(new List<string>() { "GenerateNew2FARecoveryCodes_Code" });
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository.Setup(repo => repo.GenerateNew2FARecoveryCodes(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(new Core.DTO.GatewayResponses.Repositories.GenerateNew2FARecoveryCodesResponse(Guid.Parse(appUser.Id), new List<string>() { "GenerateNew2FARecoveryCodes_Code" }, true));
        var mockLogger = new Mock<ILogger<GenerateNew2FARecoveryCodesUseCase>>();
        var useCase = new GenerateNew2FARecoveryCodesUseCase(mockUserRepository.Object);
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act
        var response = await useCase.Handle(new GenerateNew2FARecoveryCodesRequest("id", 123), mockOutputPort.Object);

        // assert
        Assert.True(response);
        mockUserRepository.Verify(factory => factory.CheckPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockOutputPort.VerifyAll();
    }
    [Fact]
    public async Task Handle_GivenINValidCredentials_ShouldFail()
    {
        // arrange
        AppUser appUser = new AppUser("", "", "", "");
        User user = new User();
        List<Claim> claims = new List<Claim>();
        Mock<UserManager<AppUser>> userManager = new Mock<UserManager<AppUser>>();
        userManager.Setup(i => i.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(appUser);
        userManager.Setup(i => i.GenerateNewTwoFactorRecoveryCodesAsync(It.IsAny<AppUser>(), It.IsAny<int>())).ReturnsAsync(new List<string>() { });
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository.Setup(repo => repo.GenerateNew2FARecoveryCodes(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(new Core.DTO.GatewayResponses.Repositories.GenerateNew2FARecoveryCodesResponse(Guid.Parse(appUser.Id), new List<string>() { }, false));

        var mockLogger = new Mock<ILogger<GenerateNew2FARecoveryCodesUseCase>>();
        var useCase = new GenerateNew2FARecoveryCodesUseCase(mockUserRepository.Object);
        var mockOutputPort = new Mock<IOutputPort<UseCaseResponseMessage>>();
        mockOutputPort.Setup(outputPort => outputPort.Handle(It.IsAny<UseCaseResponseMessage>()));

        // act
        var response = await useCase.Handle(new GenerateNew2FARecoveryCodesRequest("id", 123), mockOutputPort.Object);

        // assert
        Assert.False(response);
        mockOutputPort.VerifyAll();
    }
}