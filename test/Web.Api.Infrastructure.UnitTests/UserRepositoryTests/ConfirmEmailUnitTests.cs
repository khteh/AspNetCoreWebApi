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
using Web.Api.Core.DTO.GatewayResponses.Repositories;
using Xunit;
namespace Web.Api.Infrastructure.UnitTests.UserRepository;
public class ConfirmEmailUnitTests : IClassFixture<InfrastructureTestBase>
{
    private InfrastructureTestBase _fixture;
    public ConfirmEmailUnitTests(InfrastructureTestBase fixture) => _fixture = fixture;
    [Fact(Skip = "Call to getUser() will throw null reference exception")]
    public async void GivenValidCredentials_ShouldSucceed()
    {
        // arrange
        AppUser appUser = new AppUser("", "", "", "");
        User user = new User();
        List<Claim> claims = new List<Claim>();
        _fixture.UserManager.Setup(i => i.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(appUser);
        _fixture.UserManager.Setup(i => i.ConfirmEmailAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

        // act
        FindUserResponse response = await _fixture.UserRepository.ConfirmEmail("id", "code");

        // assert
        Assert.True(response.Success);
        Assert.Null(response.Errors);
        _fixture.UserManager.VerifyAll();
    }
    [Fact]
    public async void GivenInValidCode_ShouldFail()
    {
        // arrange
        AppUser appUser = new AppUser("", "", "", "");
        User user = new User();
        List<Claim> claims = new List<Claim>();
        _fixture.UserManager.Setup(i => i.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(appUser);
        _fixture.UserManager.Setup(i => i.ConfirmEmailAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(new IdentityError[] { new IdentityError() { Code = "123", Description = "Fail" } }));

        // act
        FindUserResponse response = await _fixture.UserRepository.ConfirmEmail("id", "code");

        // assert
        Assert.False(response.Success);
        Assert.NotNull(response.Errors);
        Assert.NotEmpty(response.Errors);
        _fixture.UserManager.VerifyAll();
    }
}