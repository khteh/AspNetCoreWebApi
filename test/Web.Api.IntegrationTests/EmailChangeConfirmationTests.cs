using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Web.Api.Infrastructure.Identity;
using Xunit;

namespace Web.Api.IntegrationTests;
[Collection("EmailChangeConfirmation Test Collection")]
public class EmailChangeConfirmationTests
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    public EmailChangeConfirmationTests(CustomWebApplicationFactory<Program> factory) => _factory = factory;
    [Fact]
    public async Task ChangeEmailShouldSucceedTest()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            UserManager<AppUser> userManager = scopedServices.GetRequiredService<UserManager<AppUser>>();
            Assert.NotNull(userManager);
            AppUser user = await userManager.FindByNameAsync("deleteme");
            Assert.NotNull(user);
            string code = await userManager.GenerateChangeEmailTokenAsync(user, "deleteme@me.com");
            IdentityResult identityResult = await userManager.ChangeEmailAsync(user, "deleteme@me.com", code);
            Assert.True(identityResult.Succeeded);
        }
    }
    [Fact]
    public async Task ChangeEmailShouldFailTest()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            UserManager<AppUser> userManager = scopedServices.GetRequiredService<UserManager<AppUser>>();
            Assert.NotNull(userManager);
            AppUser user = await userManager.FindByNameAsync("mickeymouse");
            Assert.NotNull(user);
            string code = await userManager.GenerateChangeEmailTokenAsync(user, "mickey_new@mouse.com");
            IdentityResult identityResult = await userManager.ChangeEmailAsync(user, "mickey_should_fail@mouse.com", code);
            Assert.False(identityResult.Succeeded);
        }
    }
}