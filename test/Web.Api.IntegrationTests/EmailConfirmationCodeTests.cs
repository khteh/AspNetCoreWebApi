using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using System.Threading.Tasks;
using Web.Api.Infrastructure.Identity;
using Xunit;

namespace Web.Api.IntegrationTests;
//[Collection(EmailConfirmationCodeTestsCollection.Name)]
public class EmailConfirmationCodeTests
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    public EmailConfirmationCodeTests(CustomWebApplicationFactory<Program> factory) => _factory = factory;
    [Fact]
    public async Task EmailConfirmationTokenShoudMatchTest()
    {
        using (var scope = _factory.Services.CreateScope())
            try
            {
                var scopedServices = scope.ServiceProvider;
                UserManager<AppUser> um = scopedServices.GetRequiredService<UserManager<AppUser>>();
                AppUser user = await um.FindByNameAsync("testuser");
                Assert.NotNull(user);
                string code = await um.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                IdentityResult identityResult = await um.ConfirmEmailAsync(user, code);
                Assert.True(identityResult.Succeeded);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{nameof(EmailConfirmationTokenShoudMatchTest)} exception! {ex}");
                throw;
            }
    }
    [Fact]
    public async Task EmailConfirmationTokenShoudNOTMatchTest()
    {
        using (var scope = _factory.Services.CreateScope())
            try
            {
                var scopedServices = scope.ServiceProvider;
                UserManager<AppUser> um = scopedServices.GetRequiredService<UserManager<AppUser>>();
                AppUser user = await um.FindByNameAsync("testuser");
                Assert.NotNull(user);
                string code = await um.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                AppUser user1 = await um.FindByNameAsync("deleteme");
                Assert.NotNull(user1);
                IdentityResult identityResult = await um.ConfirmEmailAsync(user1, code);
                Assert.False(identityResult.Succeeded);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{nameof(EmailConfirmationTokenShoudNOTMatchTest)} exception! {ex}");
                throw;
            }
    }
}