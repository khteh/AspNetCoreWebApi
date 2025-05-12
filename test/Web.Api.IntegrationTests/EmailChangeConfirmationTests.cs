using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Web.Api.Infrastructure.Identity;
using Web.Api.IntegrationTests.Controllers;
using Xunit;

namespace Web.Api.IntegrationTests;
[Collection(EmailChangeConfirmationTestsCollection.Name)]
public class EmailChangeConfirmationTests
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    public EmailChangeConfirmationTests(CustomWebApplicationFactory<Program> factory) => _factory = factory;
    [Fact]
    public async Task ChangeEmailShouldSucceedTest()
    {
        Assert.NotNull(_factory.UserManager);
        AppUser user = await _factory.UserManager.FindByNameAsync("deleteme");
        Assert.NotNull(user);
        string code = await _factory.UserManager.GenerateChangeEmailTokenAsync(user, "deleteme@me.com");
        IdentityResult identityResult = await _factory.UserManager.ChangeEmailAsync(user, "deleteme@me.com", code);
        Assert.True(identityResult.Succeeded);
    }
    [Fact]
    public async Task ChangeEmailShouldFailTest()
    {
        Assert.NotNull(_factory.UserManager);
        AppUser user = await _factory.UserManager.FindByNameAsync("mickeymouse");
        Assert.NotNull(user);
        string code = await _factory.UserManager.GenerateChangeEmailTokenAsync(user, "mickey_new@mouse.com");
        IdentityResult identityResult = await _factory.UserManager.ChangeEmailAsync(user, "mickey_should_fail@mouse.com", code);
        Assert.False(identityResult.Succeeded);
    }
}