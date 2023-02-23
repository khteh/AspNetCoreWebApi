using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Web.Api.Infrastructure.Identity;
using Xunit;

namespace Web.Api.IntegrationTests;
[Collection("Controller Test Collection")]
public class EmailConfirmationCodeTests
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    public EmailConfirmationCodeTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        factory.InitDB();
    }
    [Fact]
    public async Task EmailConfirmationTokenShoudMatchTest()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            UserManager<AppUser> userManager = scopedServices.GetRequiredService<UserManager<AppUser>>();
            Assert.NotNull(userManager);
            AppUser user = await userManager.FindByNameAsync("mickeymouse");
            Assert.NotNull(user);
            string code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            IdentityResult identityResult = await userManager.ConfirmEmailAsync(user, code);
            Assert.True(identityResult.Succeeded);
        }
    }
    [Fact]
    public async Task EmailConfirmationTokenShoudNOTMatchTest()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            UserManager<AppUser> userManager = scopedServices.GetRequiredService<UserManager<AppUser>>();
            Assert.NotNull(userManager);
            AppUser user = await userManager.FindByNameAsync("mickeymouse");
            Assert.NotNull(user);
            string code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            AppUser user1 = await userManager.FindByNameAsync("user1");
            Assert.NotNull(user1);
            IdentityResult identityResult = await userManager.ConfirmEmailAsync(user1, code);
            Assert.True(identityResult.Succeeded);
        }
    }
}