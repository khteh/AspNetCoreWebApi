using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Web.Api.Infrastructure.Identity;
using Xunit;

namespace Web.Api.IntegrationTests;
//[Collection(EmailChangeConfirmationTestsCollection.Name)]
public class EmailChangeConfirmationTests
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    public EmailChangeConfirmationTests(CustomWebApplicationFactory<Program> factory) => _factory = factory;
    [Fact]
    public async Task ChangeEmailShouldSucceedTest()
    {
        using (var scope = _factory.Services.CreateScope())
            try
            {
                var scopedServices = scope.ServiceProvider;
                UserManager<AppUser> um = scopedServices.GetRequiredService<UserManager<AppUser>>();
                AppUser user = await um.FindByNameAsync("deleteme");
                Assert.NotNull(user);
                string code = await um.GenerateChangeEmailTokenAsync(user, "deleteme@me.com");
                IdentityResult identityResult = await um.ChangeEmailAsync(user, "deleteme@me.com", code);
                Assert.True(identityResult.Succeeded);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{nameof(ChangeEmailShouldSucceedTest)} exception! {ex}");
                throw;
            }
    }
    [Fact]
    public async Task ChangeEmailShouldFailTest()
    {
        using (var scope = _factory.Services.CreateScope())
            try
            {
                var scopedServices = scope.ServiceProvider;
                UserManager<AppUser> um = scopedServices.GetRequiredService<UserManager<AppUser>>();
                AppUser user = await um.FindByNameAsync("testuser");
                Assert.NotNull(user);
                string code = await um.GenerateChangeEmailTokenAsync(user, "testuser_new@email.com");
                IdentityResult identityResult = await um.ChangeEmailAsync(user, "testuser_should_fail@email.com", code);
                Assert.False(identityResult.Succeeded);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{nameof(ChangeEmailShouldFailTest)} exception! {ex}");
                throw;
            }
    }
}