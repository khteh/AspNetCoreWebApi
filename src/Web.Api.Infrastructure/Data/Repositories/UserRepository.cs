using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.GatewayResponses.Repositories;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.Services;
using Web.Api.Core.Specifications;
using Web.Api.Infrastructure.Identity;
namespace Web.Api.Infrastructure.Data.Repositories;
public sealed class UserRepository : EfRepository<User>, IUserRepository
{
    private readonly IJwtFactory _jwtFactory;
    private readonly IJwtTokenValidator _jwtTokenValidator;
    private readonly ITokenFactory _tokenFactory;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IMapper _mapper;
    private readonly ILogger<UserRepository> _logger;
    public UserRepository(ILogger<UserRepository> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper, AppDbContext appDbContext, IJwtFactory jwtFactory, IJwtTokenValidator jwtTokenValidator, ITokenFactory tokenFactory) : base(appDbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtFactory = jwtFactory;
        _jwtTokenValidator = jwtTokenValidator;
        _tokenFactory = tokenFactory;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<CreateUserResponse> Create(string firstName, string lastName, string email, string userName, string password)
    {
        try
        {
            var appUser = new AppUser { Email = email, UserName = userName, FirstName = firstName, LastName = lastName };
            var identityResult = await _userManager.CreateAsync(appUser, password);
            if (!identityResult.Succeeded)
                return new CreateUserResponse(appUser.Id, false, identityResult.Errors.Select(e => new Error(e.Code, e.Description)).ToList());
            // add the email claim and value for this user
            await _userManager.AddClaimAsync(appUser, new Claim(ClaimTypes.Name, appUser.UserName));
            var user = new User(firstName, lastName, appUser.Id, appUser.UserName, appUser.Email, appUser.PhoneNumber);
            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync();
            return new CreateUserResponse(appUser.Id, identityResult.Succeeded, identityResult.Succeeded ? null : identityResult.Errors.Select(e => new Error(e.Code, e.Description)).ToList());
        }
        catch (Exception e)
        {
            _logger.LogCritical($"{nameof(UserRepository)}.{nameof(Create)} Exception! {e.Message}");
            return new CreateUserResponse(null, false, new List<Error>() { new Error(HttpStatusCode.InternalServerError.ToString(), e.Message) });
        }
    }
    public async Task<DeleteUserResponse> Delete(string userName)
    {
        try
        {
            AppUser appUser = await _userManager.FindByNameAsync(userName);
            if (appUser != null)
            {
                var identityResult = await _userManager.DeleteAsync(appUser);
                if (!identityResult.Succeeded)
                    return new DeleteUserResponse(appUser.Id, false, identityResult.Errors.Select(e => new Error(e.Code, e.Description)).ToList());
                User user = await getUser(appUser);
                if (user != null)
                {
                    _appDbContext.Users.Remove(user);
                    await _appDbContext.SaveChangesAsync();
                }
                else
                    return new DeleteUserResponse(null, false, new List<Error>() { new Error(HttpStatusCode.InternalServerError.ToString(), "Failed to remove user from app DB! This is result in inconsistency between application Users table and Identity framework!") });
                return new DeleteUserResponse(appUser.Id, identityResult.Succeeded, identityResult.Succeeded ? null : identityResult.Errors.Select(e => new Error(e.Code, e.Description)).ToList());
            }
            else
                return new DeleteUserResponse(null, false, new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), "Invalid user!") });
        }
        catch (Exception e)
        {
            _logger.LogCritical($"{nameof(UserRepository)}.{nameof(Delete)} Exception! {e.Message}");
            return new DeleteUserResponse(null, false, new List<Error>() { new Error(HttpStatusCode.InternalServerError.ToString(), e.Message) });
        }
    }
    public async Task<ExchangeRefreshTokenResponse> ExchangeRefreshToken(string accessToken, string refreshToken, string signingKey)
    {
        ExchangeRefreshTokenResponse response = null;
        try
        {
            ClaimsPrincipal cp = _jwtTokenValidator.GetPrincipalFromToken(accessToken, signingKey);
            // invalid token/signing key was passed and we can't extract user claims
            if (cp != null)
            {
                Claim claim = cp.Claims.First(c => c.Type == "id");
                User user = await getUser(await _userManager.FindByIdAsync(claim.Value));
                if (user != null && user.HasValidRefreshToken(refreshToken))
                {
                    AccessToken jwtToken = await _jwtFactory.GenerateEncodedToken(user.IdentityId, user.UserName);
                    string newRefreshToken = _tokenFactory.GenerateToken();
                    user.RemoveRefreshToken(refreshToken); // delete the token we've exchanged
                    user.AddRefreshToken(newRefreshToken, ""); // add the new one
                    await Update(user);
                    response = new ExchangeRefreshTokenResponse(jwtToken, refreshToken, true);
                }
                else if (user == null)
                    return new ExchangeRefreshTokenResponse(null, null, false, new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), "Invalid user!") });
            }
            else
                return new ExchangeRefreshTokenResponse(null, null, false, new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), "Invalid credential!") });
        }
        catch (Exception e)
        {
            _logger.LogCritical($"{nameof(UserRepository)}.{nameof(ExchangeRefreshToken)} Exception! {e.Message}");
            return new ExchangeRefreshTokenResponse(null, null, false, new List<Error>() { new Error(HttpStatusCode.InternalServerError.ToString(), e.Message) });
        }
        return response;
    }
    public async Task<User> FindUserByName(string userName) => await getUser(await _userManager.FindByNameAsync(userName));
    public async Task<FindUserResponse> FindByName(string userName) => await getFindUserResponse(await _userManager.FindByNameAsync(userName));
    public async Task<FindUserResponse> FindById(string id) => await getFindUserResponse(await _userManager.FindByIdAsync(id));
    public async Task<FindUserResponse> FindByEmail(string email) => await getFindUserResponse(await _userManager.FindByEmailAsync(email));
    /// <summary>
    /// SignIn - Requires AddCookie() in startup.cs
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="rememberMe"></param>
    /// <param name="logoutOnFailure"></param>
    /// <returns></returns>
    public async Task<SignInResponse> SignInMobile(string username, string password, bool logoutOnFailure)
    {
        try
        {
            AppUser user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(SignInMobile)} Invalid username {username} and password {password}!");
                return new SignInResponse(Guid.Empty, null, false, false, false, new List<Error>() { new Error("NotSucceeded", $"Invalid username {username} and password {password}!") });
            }
            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, password, logoutOnFailure); ;
            if (result.IsNotAllowed)
            {
                _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(SignInMobile)} User account {username} is not allowed to login!");
                return new SignInResponse(Guid.Empty, null, false, false, false, new List<Error>() { new Error("IsNotAllowed", $"User account {username} is not allowed to login!") });
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(SignInMobile)} User account {username} locked out!");
                return new SignInResponse(Guid.Empty, null, false, false, true, new List<Error>() { new Error("IsLockedOut", $"User account {username} locked out!") });
            }
            if (result.RequiresTwoFactor)
            {
                _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(SignInMobile)} User account {username} requires two-factor authentication!");
                return new SignInResponse(Guid.Empty, null, false, true, false, new List<Error>() { new Error("RequiresTwoFactor", $"User account {username} requires two-factor authentication!") });
            }
            if (!result.Succeeded)
            {
                _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(SignInMobile)} Invalid username {username} and password {password}!");
                return new SignInResponse(Guid.Empty, null, false, false, false, new List<Error>() { new Error("NotSucceeded", $"Invalid username {username} and password {password}!") });
            }
            // Use _userManager.IsInRoleAsync(user,  to check if user is in the required role
            _logger.LogInformation(2, $"{nameof(UserRepository)}.{nameof(SignInMobile)} User {username} signed in successfully!");
            return new SignInResponse(Guid.Parse(user.Id), user.UserName, true);
        }
        catch (Exception e)
        {
            _logger.LogCritical(2, $"{nameof(UserRepository)}.{nameof(SignInMobile)} Exception! {e.Message}");
            return new SignInResponse(Guid.Empty, null, false, false, false, new List<Error>() { new Error("NotSucceeded", $"Exception! {e.Message}") });
        }
    }
    public async Task<SignInResponse> SignIn(string username, string password, string remoteIP, bool rememberMe, bool logoutOnFailure)
    {
        try
        {
            AppUser user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(SignIn)} Invalid username {username} and password {password} from remoteIP: {remoteIP}!");
                return new SignInResponse(Guid.Empty, null, false, false, false, new List<Error>() { new Error(HttpStatusCode.Forbidden.ToString(), $"Invalid username {username} and password {password}!") });
            }
            SignInResult result = await _signInManager.PasswordSignInAsync(username, password, rememberMe, logoutOnFailure);
            if (result.IsNotAllowed)
            {
                _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(SignIn)} User account {username} is not allowed to login from remoteIP: {remoteIP}!");
                return new SignInResponse(Guid.Empty, null, false, false, false, new List<Error>() { new Error("IsNotAllowed", $"User account {username} is not allowed to login!") });
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(SignIn)} User account {username} locked out from remoteIP: {remoteIP}!");
                return new SignInResponse(Guid.Empty, null, false, false, true, new List<Error>() { new Error("IsLockedOut", $"User account {username} locked out!") });
            }
            if (result.RequiresTwoFactor)
            {
                _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(SignIn)} User account {username} requires two-factor authentication from remoteIP: {remoteIP}!");
                return new SignInResponse(Guid.Empty, null, false, true, false, new List<Error>() { new Error("RequiresTwoFactor", $"User account {username} requires two-factor authentication!") });
            }
            if (!result.Succeeded)
            {
                _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(SignIn)} Invalid username {username} and password {password} from remoteIP: {remoteIP}!");
                return new SignInResponse(Guid.Empty, null, false, false, false, new List<Error>() { new Error(HttpStatusCode.Forbidden.ToString(), $"Invalid username {username} and password {password}!") });
            }
            _logger.LogInformation(2, $"{nameof(UserRepository)}.{nameof(SignIn)} User {username} signed in successfully from remoteIP: {remoteIP}!");
            return new SignInResponse(Guid.Parse(user.Id), user.UserName, true);
        }
        catch (Exception e)
        {
            _logger.LogCritical(2, $"{nameof(UserRepository)}.{nameof(SignIn)} Exception! {e.Message}");
            return new SignInResponse(Guid.Empty, null, false, false, false, new List<Error>() { new Error(HttpStatusCode.InternalServerError.ToString(), $"Exception! {e.Message}") });
        }
    }
    public async Task<SignInResponse> SignInWithClaims(string identityId, List<Claim> claims, AuthenticationProperties authProperties)
    {
        try
        {
            AppUser user = _mapper.Map<User, AppUser>(await GetSingleBySpec(new UserSpecification(identityId)));
            if (user != null)
            {
                await _signInManager.SignInWithClaimsAsync(user, authProperties, claims);
                _logger.LogInformation(2, $"{nameof(UserRepository)}.{nameof(SignInWithClaims)} User {user.UserName} signed in successfully!");
                return new SignInResponse(Guid.Parse(user.Id), user.UserName, true);
            }
            _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(SignInWithClaims)} user {identityId} sign in failed!");
            return new SignInResponse(Guid.Empty, null, false, false, false, new List<Error>() { new Error(HttpStatusCode.Forbidden.ToString(), $"Invalid identityId {identityId}!") });
        }
        catch (Exception e)
        {
            _logger.LogCritical(2, $"{nameof(UserRepository)}.{nameof(SignInWithClaims)} Exception! {e.Message}");
            return new SignInResponse(Guid.Empty, null, false, false, false, new List<Error>() { new Error(HttpStatusCode.InternalServerError.ToString(), $"Exception! {e.Message}") });
        }
    }
    public async Task<SignInResponse> TwoFactorRecoveryCodeSignIn(string recoveryCode)
    {
        try
        {
            FindUserResponse response = await GetTwoFactorAuthenticationUserAsync();
            if (!response.Success || string.IsNullOrEmpty(response.Id) || response.User == null || response.Errors.Any())
            {
                _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(TwoFactorRecoveryCodeSignIn)} Invalid {recoveryCode}!");
                return new SignInResponse(Guid.Empty, null, false, false, false, new List<Error>() { new Error("IsNotAllowed", $"Invalid {recoveryCode}!") });
            }
            SignInResult result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);
            if (result.IsNotAllowed)
            {
                _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(TwoFactorRecoveryCodeSignIn)} {recoveryCode} is not allowed to login!");
                return new SignInResponse(Guid.Empty, null, false, false, false, new List<Error>() { new Error("IsNotAllowed", $"Invalid {recoveryCode}!") });
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(TwoFactorRecoveryCodeSignIn)} {recoveryCode} locked out!");
                return new SignInResponse(Guid.Empty, null, false, false, true, new List<Error>() { new Error("IsLockedOut", $"Invalid {recoveryCode}!") });
            }
            if (!result.Succeeded)
            {
                _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(TwoFactorRecoveryCodeSignIn)} {recoveryCode}!");
                return new SignInResponse(Guid.Empty, null, false, false, false, new List<Error>() { new Error("NotSucceeded", $"Invalid {recoveryCode}!") });
            }
            _logger.LogInformation(2, $"{nameof(UserRepository)}.{nameof(TwoFactorRecoveryCodeSignIn)} RecoveryCode {recoveryCode} signed in successfully!");
            return new SignInResponse(Guid.Parse(response.Id), response.User.UserName, true);
        }
        catch (Exception e)
        {
            _logger.LogCritical(2, $"{nameof(UserRepository)}.{nameof(TwoFactorRecoveryCodeSignIn)} Exception! {e.Message}");
            return new SignInResponse(Guid.Empty, null, false, false, false, new List<Error>() { new Error(HttpStatusCode.InternalServerError.ToString(), $"Exception! {e.Message}") });
        }
    }
    public async Task<GenerateNew2FARecoveryCodesResponse> GenerateNew2FARecoveryCodes(string id, int codes)
    {
        try
        {
            AppUser appUser = await _userManager.FindByIdAsync(id);
            if (appUser != null)
                return new GenerateNew2FARecoveryCodesResponse(id, (await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(appUser, codes)).ToList(), true);
            return new GenerateNew2FARecoveryCodesResponse(string.Empty, null, false, new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), $"Invalid user {id}!") });
        }
        catch (Exception e)
        {
            _logger.LogCritical($"{nameof(UserRepository)}.{nameof(GenerateNew2FARecoveryCodes)} Exception! {e}");
            return new GenerateNew2FARecoveryCodesResponse(string.Empty, null, false, new List<Error>() { new Error(HttpStatusCode.InternalServerError.ToString(), e.Message) });
        }
    }
    public async Task<LogInResponse> CheckPassword(string username, string password)
    {
        try
        {
            AppUser user = await _userManager.FindByNameAsync(username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            {
                _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(CheckPassword)} Invalid username {username} and password {password}!");
                return new LogInResponse(null, false, new List<Error>() { new Error(HttpStatusCode.Unauthorized.ToString(), $"Invalid username or password!") });
            }
            else if (user == null)
                _logger.LogError($"{nameof(CheckPassword)} Invalid user! {username}");
            else
                _logger.LogError($"{nameof(CheckPassword)} Invalid credentials! {username}");
            return new LogInResponse(await FindUserByName(username), true);
        }
        catch (Exception e)
        {
            _logger.LogCritical(2, $"{nameof(UserRepository)}.{nameof(CheckPassword)} Exception! {e}");
            return new LogInResponse(null, false, new List<Error>() { new Error(HttpStatusCode.InternalServerError.ToString(), $"Exception! {e}") });
        }
    }
    public async Task<PasswordResponse> ChangePassword(string id, string oldPassword, string newPassword)
    {
        try
        {
            AppUser appUser = await _userManager.FindByIdAsync(id);
            if (appUser == null)
                return new PasswordResponse(null, false, new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), "User not found!") });
            IdentityResult identityResult = await _userManager.ChangePasswordAsync(appUser, oldPassword, newPassword);
            if (identityResult.Succeeded)
                return new PasswordResponse(appUser.Id, true, null);
            else
            {
                _logger.LogError($"{nameof(ChangePassword)} failed to change password of user {id}!");
                return new PasswordResponse(appUser.Id, false, identityResult.Errors.Select(e => new Error(e.Code, e.Description)).ToList());
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"{nameof(ChangePassword)} Exception! {e.Message}");
            return new PasswordResponse(null, false, new List<Error>() { new Error(HttpStatusCode.InternalServerError.ToString(), $"User change password failed! {e.Message}") });
        }
    }
    public async Task<PasswordResponse> ResetPassword(string id, string password)
    {
        AppUser user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            IdentityResult result = await _userManager.ResetPasswordAsync(user, await _userManager.GeneratePasswordResetTokenAsync(user), password);
            if (result.Succeeded)
            {
                result = await _userManager.ResetAccessFailedCountAsync(user);
                if (!result.Succeeded)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (IdentityError error in result.Errors)
                        sb.AppendLine($"{error.Code}: {error.Description}");
                    _logger.LogError($"{nameof(ResetPassword)} failed to reset access failed count of user {user.Id}! {sb.ToString()}");
                    return new PasswordResponse(user.Id, result.Succeeded, result.Succeeded ? null : result.Errors.Select(e => new Error(e.Code, e.Description)).ToList());
                }
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (IdentityError error in result.Errors)
                    sb.AppendLine($"{error.Code}: {error.Description}");
                _logger.LogError($"{nameof(ResetPassword)} failed to reset password of user {user.Id}!");
                return new PasswordResponse(user.Id, result.Succeeded, result.Succeeded ? null : result.Errors.Select(e => new Error(e.Code, e.Description)).ToList());
            }
            return new PasswordResponse(user.Id, result.Succeeded, result.Succeeded ? null : result.Errors.Select(e => new Error(e.Code, e.Description)).ToList());
        }
        else
        {
            _logger.LogError($"Trying to reset password of  invalid user {id}!");
            return new PasswordResponse(null, false, new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), $"Trying to reset password of invalid user {id}!") });
        }
    }
    public async Task<PasswordResponse> ResetPassword(string email, string password, string code)
    {
        AppUser user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            IdentityResult result = await _userManager.ResetPasswordAsync(user, code, password);
            if (result.Succeeded)
            {
                result = await _userManager.ResetAccessFailedCountAsync(user);
                if (!result.Succeeded)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (IdentityError error in result.Errors)
                        sb.AppendLine($"{error.Code} {error.Description}");
                    _logger.LogError($"{nameof(ResetPassword)} failed to reset access failed count of user {user.Id}! {sb.ToString()}");
                    return new PasswordResponse(user.Id, result.Succeeded, result.Succeeded ? null : result.Errors.Select(e => new Error(e.Code, e.Description)).ToList());
                }
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (IdentityError error in result.Errors)
                    sb.AppendLine($"{error.Code} {error.Description}");
                _logger.LogError($"{nameof(ResetPassword)} failed to reset password of user {user.Id}! {sb.ToString()}");
                return new PasswordResponse(user.Id, result.Succeeded, result.Succeeded ? null : result.Errors.Select(e => new Error(e.Code, e.Description)).ToList());
            }
            return new PasswordResponse(user.Id, result.Succeeded, result.Succeeded ? null : result.Errors.Select(e => new Error(e.Code, e.Description)).ToList());
        }
        else
        {
            _logger.LogError($"Trying to reset password of  invalid user {email}!");
            return new PasswordResponse(string.Empty, false, new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), $"Trying to reset password of invalid user {email}!") });
        }
    }
    public async Task<LockUserResponse> LockUser(string id) => await LockUser(await _userManager.FindByIdAsync(id));
    public async Task<LockUserResponse> LockUser(int id)
    {
        User user = await _appDbContext.Users.FindAsync(id);
        return user == null ?
                new LockUserResponse(null, false, new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), $"Invalid user {id}!") })
                : await LockUser(await _userManager.FindByIdAsync(user.IdentityId));
    }
    private async Task<LockUserResponse> LockUser(AppUser user)
    {
        if (user != null)
        {
            if (user.LockoutEnabled && user.LockoutEnd > DateTimeOffset.UtcNow)
            {
                _logger.LogCritical($"Trying to lock an already locked user {user.Id}!");
                return new LockUserResponse(null, false, new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), $"Trying to lock an already locked user {user.Id}!") });
            }
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;
            IdentityResult result = await _userManager.UpdateAsync(user);
            return new LockUserResponse(user.Id, result.Succeeded, result.Succeeded ? null : result.Errors.Select(e => new Error(e.Code, e.Description)).ToList());
        }
        else
        {
            _logger.LogCritical($"Trying to lock an invalid user {user.Id}!");
            return new LockUserResponse(null, false, new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), $"Trying to lock an invalid user {user.Id}!") });
        }
    }
    public async Task<LockUserResponse> UnLockUser(string id) => await UnLockUser(await _userManager.FindByIdAsync(id));
    public async Task<LockUserResponse> UnLockUser(int id)
    {
        User user = await _appDbContext.Users.FindAsync(id);
        return user == null ?
                new LockUserResponse(null, false, new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), $"Invalid user {id}!") })
                : await UnLockUser(await _userManager.FindByIdAsync(user.IdentityId));
    }
    public async Task<CodeResponse> RegistrationConfirmation(string email)
    {
        try
        {
            AppUser user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogError($"{nameof(UserRepository)}.{nameof(RegistrationConfirmation)} Invalid user {email}!");
                return new CodeResponse(string.Empty, null, false, new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), $"Cannot reset password of invalid user {email}!") });
            }
            Core.Domain.Entities.User u = await GetSingleBySpec(new UserSpecification(user.Id));
            if (u == null)
            {
                _logger.LogError($"{nameof(UserRepository)}.{nameof(RegistrationConfirmation)} Invalid user IdentityId {user.Id}!");
                return new CodeResponse(string.Empty, null, false, new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), $"Cannot reset password of invalid user {email}!") });
            }
            string code = string.Empty;
            if (_userManager.Options.SignIn.RequireConfirmedAccount)
            {
                code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            }
            return new CodeResponse(user.Id, code, true);
        }
        catch (Exception e)
        {
            _logger.LogCritical($"{nameof(UserRepository)}.{nameof(RegistrationConfirmation)} Exception! {e.Message}");
            return new CodeResponse(string.Empty, null, false, new List<Error>() { new Error(HttpStatusCode.InternalServerError.ToString(), e.Message) });
        }
    }
    public async Task<CodeResponse> GenerateChangeEmailToken(string identityId, string email)
    {
        try
        {
            AppUser user = await _userManager.FindByIdAsync(identityId);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                return new CodeResponse(string.Empty, null, false, new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), $"Cannot change email of invalid user {email}!") });
            // For more information on how to enable account confirmation and password reset please
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            var code = await _userManager.GenerateChangeEmailTokenAsync(user, email);
            return new CodeResponse(user.Id, code, true);
        }
        catch (Exception e)
        {
            _logger.LogCritical($"{nameof(UserRepository)}.{nameof(GenerateChangeEmailToken)} Exception! {e.Message}");
            return new CodeResponse(string.Empty, null, false, new List<Error>() { new Error(HttpStatusCode.InternalServerError.ToString(), e.Message) });
        }
    }
    public async Task<FindUserResponse> ConfirmEmail(string identityId, string code)
    {
        try
        {
            AppUser appUser = await _userManager.FindByIdAsync(identityId);
            if (appUser != null)
            {
                IdentityResult identityResult = await _userManager.ConfirmEmailAsync(appUser, code);
                return !identityResult.Succeeded ?
                    new FindUserResponse(string.Empty, null, false, identityResult.Errors.Select(e => new Error(e.Code, e.Description)).ToList())
                    : new FindUserResponse(appUser.Id, await getUser(appUser), identityResult.Succeeded, identityResult.Errors.Select(e => new Error(e.Code, e.Description)).ToList());
            }
            else
                _logger.LogError($"{nameof(UserRepository)}.{nameof(ConfirmEmail)} Invalid user email confirmation! Id: {identityId}, code: {code}");
            return new FindUserResponse(string.Empty, null, false, new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), $"Cannot add confirm email of invalid user {identityId}!") });
        }
        catch (Exception e)
        {
            _logger.LogCritical($"{nameof(UserRepository)}.{nameof(ConfirmEmail)} Exception! {e.Message}");
            return new FindUserResponse(string.Empty, null, false, new List<Error>() { new Error(HttpStatusCode.InternalServerError.ToString(), e.Message) });
        }
    }
    public async Task<FindUserResponse> ConfirmEmailChange(string identityId, string email, string code)
    {
        try
        {
            AppUser appUser = await _userManager.FindByIdAsync(identityId);
            if (appUser != null)
            {
                var identityResult = await _userManager.ChangeEmailAsync(appUser, email, code);
                if (!identityResult.Succeeded)
                    return new FindUserResponse(appUser.Id, null, identityResult.Succeeded, identityResult.Errors.Select(e => new Error(e.Code, e.Description)).ToList());
                // In our UI email and user name are one and the same, so when we update the email
                // we need to update the user name.
                var setUserNameResult = await _userManager.SetUserNameAsync(appUser, email);
                if (!identityResult.Succeeded)
                    return new FindUserResponse(appUser.Id, null, identityResult.Succeeded, identityResult.Errors.Select(e => new Error(e.Code, e.Description)).ToList());
                await _signInManager.RefreshSignInAsync(appUser);
                return new FindUserResponse(appUser.Id, await getUser(appUser), true, null);
            }
            return new FindUserResponse(string.Empty, null, false, new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), $"Cannot confirm email change of invalid user {identityId}!") });
        }
        catch (Exception e)
        {
            _logger.LogCritical($"{nameof(UserRepository)}.{nameof(ConfirmEmailChange)} Exception! {e.Message}");
            return new FindUserResponse(string.Empty, null, false, new List<Error>() { new Error(HttpStatusCode.InternalServerError.ToString(), e.Message) });
        }
    }
    private record UserModel(int Id, string FirstName, string LastName, string UserName, string Email, string PhoneNumber);
    public async Task<FindResponse<User>> FindUsers(int page = 0, int pageSize = 100)
    {
        try
        {
            List<User> users = await _appDbContext.Users.Skip(page * pageSize).Take(pageSize).ToListAsync();
            Dictionary<int, UserModel> userModels = users.Join(_userManager.Users, u => u.IdentityId, ku => ku.Id, (u, ku) => new UserModel(u.Id, ku.FirstName, ku.LastName, ku.UserName, ku.Email, ku.PhoneNumber)).ToDictionary(i => i.Id);
            users.AsParallel().ForAll(u =>
            {
                u.FirstName = userModels[u.Id].FirstName;
                u.LastName = userModels[u.Id].LastName;
                u.UserName = userModels[u.Id].UserName;
                u.Email = userModels[u.Id].Email;
                u.PhoneNumber = userModels[u.Id].PhoneNumber;
            });
            return new FindResponse<User>(Guid.Empty, users, users.Any());
        }
        catch (Exception e)
        {
            _logger.LogCritical($"{nameof(FindUsers)} exception! {e}");
            return new FindResponse<User>(Guid.Empty, null, false, new List<Error>() { new Error(HttpStatusCode.InternalServerError.ToString(), $"{nameof(FindUsers)} exception! {e}") });
        }
    }
    private async Task<LockUserResponse> UnLockUser(AppUser user)
    {
        if (user != null)
        {
            if (!user.LockoutEnabled || !user.LockoutEnd.HasValue || user.LockoutEnd < DateTimeOffset.UtcNow)
            {
                _logger.LogCritical($"Trying to unlock an unlocked user {user.Id}!");
                return new LockUserResponse(null, false, new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), $"Trying to unlock an unlocked user {user.Id}!") });
            }
            user.LockoutEnabled = true;
            user.LockoutEnd = null;
            user.AccessFailedCount = 0;
            IdentityResult result = await _userManager.UpdateAsync(user);
            return new LockUserResponse(user.Id, result.Succeeded, result.Succeeded ? null : result.Errors.Select(e => new Error(e.Code, e.Description)).ToList());
        }
        else
        {
            _logger.LogCritical($"Trying to unlock an invalid user {user.Id}!");
            return new LockUserResponse(null, false, new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), $"Trying to unlock an invalid user {user.Id}!") });
        }
    }
    private async Task<User> getUser(AppUser appUser) => appUser == null ? null : _mapper.Map<AppUser, User>(appUser, await GetSingleBySpec(new UserSpecification(appUser.Id)));
    private async Task<FindUserResponse> getFindUserResponse(AppUser appUser)
    {
        User user = await getUser(appUser);
        return user == null ? new FindUserResponse(null, null, false, new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), "User not found!") }) :
                new FindUserResponse(appUser.Id, user, true, null);
    }
    public async Task<FindUserResponse> GetTwoFactorAuthenticationUserAsync() => await getFindUserResponse(await _signInManager.GetTwoFactorAuthenticationUserAsync());
}