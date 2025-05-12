using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Threading.Tasks;
using Web.Api.Infrastructure.Identity;
using Web.Api.IntegrationTests.Controllers;
using Xunit;

namespace Web.Api.IntegrationTests;
[Collection(EmailConfirmationCodeTestsCollection.Name)]
public class EmailConfirmationCodeTests
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    public EmailConfirmationCodeTests(CustomWebApplicationFactory<Program> factory) => _factory = factory;
    [Fact]
    public async Task EmailConfirmationTokenShoudMatchTest()
    {
        Assert.NotNull(_factory.UserManager);
        AppUser user = await _factory.UserManager.FindByNameAsync("mickeymouse");
        Assert.NotNull(user);
        string code = await _factory.UserManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        IdentityResult identityResult = await _factory.UserManager.ConfirmEmailAsync(user, code);
        Assert.True(identityResult.Succeeded);
    }
    [Fact]
    public async Task EmailConfirmationTokenShoudNOTMatchTest()
    {
        Assert.NotNull(_factory.UserManager);
        AppUser user = await _factory.UserManager.FindByNameAsync("mickeymouse");
        Assert.NotNull(user);
        string code = await _factory.UserManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        AppUser user1 = await _factory.UserManager.FindByNameAsync("deleteme");
        Assert.NotNull(user1);
        IdentityResult identityResult = await _factory.UserManager.ConfirmEmailAsync(user1, code);
        Assert.False(identityResult.Succeeded);
    }
}