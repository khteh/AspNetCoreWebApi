using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Claims;
using System.Security.Permissions;
using System.Threading.Tasks;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.DTO.GatewayResponses.Repositories;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.UseCases;
using Web.Api.Infrastructure.Identity;
using Xunit;
namespace Web.Api.Infrastructure.UnitTests.UserRepository;

public class ChangePasswordUnitTests : IClassFixture<InfrastructureTestBase>
{
    private InfrastructureTestBase _fixture;

    public ChangePasswordUnitTests(InfrastructureTestBase fixture) => _fixture = fixture;

    [Fact]
    public async Task Handle_ChangePassword_ShouldSucceed()
    {
        // arrange
        AppUser appUser = new AppUser("", "", "", "");
        User user = new User();
        List<Claim> claims = new List<Claim>();
        _fixture.UserManager.Setup(i => i.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(appUser);
        _fixture.UserManager.Setup(i => i.ChangePasswordAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

        // act
        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        PasswordResponse response = await _fixture.UserRepository.ChangePassword("id", "oldPassword", "newPassword");

        // assert
        Assert.True(response.Success);
        Assert.Null(response.Errors);
        _fixture.UserManager.VerifyAll();
    }
    [Fact]
    public async Task Handle_ChangePassword_InvalidInputParams_ShouldFail()
    {
        // arrange
        AppUser appUser = new AppUser("", "", "", "");
        User user = new User();
        List<Claim> claims = new List<Claim>();
        _fixture.UserManager.Setup(i => i.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(appUser);
        _fixture.UserManager.Setup(i => i.ChangePasswordAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(new IdentityError[] { new IdentityError() { Code = "123", Description = "Fail" } }));

        // act

        // 4. We need a request model to carry data into the use case from the upper layer (UI, Controller etc.)
        PasswordResponse response = await _fixture.UserRepository.ChangePassword("id", "oldPassword", "newPassword");

        // assert
        Assert.False(response.Success);
        Assert.NotNull(response.Errors);
        Assert.NotEmpty(response.Errors);
        _fixture.UserManager.VerifyAll();
    }
}