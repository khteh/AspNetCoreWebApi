using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.GatewayResponses.Repositories;
//using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Specifications;
using Web.Api.Infrastructure.Identity;
using Microsoft.Extensions.Logging;
namespace Web.Api.Infrastructure.Data.Repositories
{
    public sealed class UserRepository : EfRepository<User>, IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserRepository> _logger;
        public UserRepository(ILogger<UserRepository> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper, AppDbContext appDbContext): base(appDbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
                var user = new User(firstName, lastName, appUser.Id.ToString(), appUser.UserName);
                _appDbContext.Users.Add(user);
                await _appDbContext.SaveChangesAsync();
                return new CreateUserResponse(appUser.Id, identityResult.Succeeded, identityResult.Succeeded ? null : identityResult.Errors.Select(e => new Error(e.Code, e.Description)).ToList());
            } catch (Exception e) {
                _logger.LogCritical($"{nameof(UserRepository)}.{nameof(Create)} Exception! {e.Message}");
                return new CreateUserResponse(null, false, new List<Error>() { new Error(null, e.Message) });
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
                    } else
                        return new DeleteUserResponse(null, false, new List<Error>() { new Error(null, "Failed to remove user from app DB! This is result in inconsistency between application Users table and Identity framework!") });
                    return new DeleteUserResponse(appUser.Id, identityResult.Succeeded, identityResult.Succeeded ? null : identityResult.Errors.Select(e => new Error(e.Code, e.Description)).ToList());
                } else
                    return new DeleteUserResponse(null, false, new List<Error>() { new Error(null, "Invalid user!") });
            }
            catch (Exception e)
            {
                _logger.LogCritical($"{nameof(UserRepository)}.{nameof(Delete)} Exception! {e.Message}");
                return new DeleteUserResponse(null, false, new List<Error>() { new Error(null, e.Message) });
            }
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
            try {
            AppUser user = await _userManager.FindByNameAsync(username);
            if (user == null) {
                _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(SignInMobile)} Invalid username {username} and password {password}!");
                return new SignInResponse(null, false, new List<Error>() {new Error("NotSucceeded", $"Invalid username {username} and password {password}!")});
            }
            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, password, logoutOnFailure);;
            if (result.IsNotAllowed)
            {
                _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(SignInMobile)} User account {username} is not allowed to login!");
                return new SignInResponse(null, false, new List<Error>() {new Error("IsNotAllowed", $"User account {username} is not allowed to login!")});
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(SignInMobile)} User account {username} locked out!");
                return new SignInResponse(null, false, new List<Error>() {new Error("IsLockedOut", $"User account {username} locked out!")});
            }
            if (result.RequiresTwoFactor)
            {
                _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(SignInMobile)} User account {username} requires two-factor authentication!");
                return new SignInResponse(null, false, new List<Error>() {new Error("RequiresTwoFactor", $"User account {username} requires two-factor authentication!")});
            }
            if (!result.Succeeded) {
                _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(SignInMobile)} Invalid username {username} and password {password}!");
                return new SignInResponse(null, false, new List<Error>() {new Error("NotSucceeded", $"Invalid username {username} and password {password}!")});
            }
            // Use _userManager.IsInRoleAsync(user,  to check if user is in the required role
            _logger.LogInformation(2, $"{nameof(UserRepository)}.{nameof(SignInMobile)} User {username} signed in successfully!");
            return new SignInResponse(user.Id, true);
            } catch (Exception e) {
                _logger.LogCritical(2, $"{nameof(UserRepository)}.{nameof(SignInMobile)} Exception! {e.Message}");
                return new SignInResponse(null, false, new List<Error>() {new Error("NotSucceeded", $"Exception! {e.Message}")});
            }
        }
        public async Task<SignInResponse> SignIn(string username, string password, bool rememberMe, bool logoutOnFailure)
        {
            try {
                AppUser user = await _userManager.FindByNameAsync(username);
                if (user == null) {
                    _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(SignIn)} Invalid username {username} and password {password}!");
                    return new SignInResponse(null, false, new List<Error>() {new Error("NotSucceeded", $"Invalid username {username} and password {password}!")});
                }
                SignInResult result = await _signInManager.PasswordSignInAsync(username, password, rememberMe, logoutOnFailure);
                if (result.IsNotAllowed)
                {
                    _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(SignIn)} User account {username} is not allowed to login!");
                    return new SignInResponse(null, false, new List<Error>() {new Error("IsNotAllowed", $"User account {username} is not allowed to login!")});
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(SignIn)} User account {username} locked out!");
                    return new SignInResponse(null, false, new List<Error>() {new Error("IsLockedOut", $"User account {username} locked out!")});
                }
                if (result.RequiresTwoFactor)
                {
                    _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(SignIn)} User account {username} requires two-factor authentication!");
                    return new SignInResponse(null, false, new List<Error>() {new Error("RequiresTwoFactor", $"User account {username} requires two-factor authentication!")});
                }
                if (!result.Succeeded) {
                    _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(SignIn)} Invalid username {username} and password {password}!");
                    return new SignInResponse(null, false, new List<Error>() {new Error("NotSucceeded", $"Invalid username {username} and password {password}!")});
                }
                _logger.LogInformation(2, $"{nameof(UserRepository)}.{nameof(SignIn)} User {username} signed in successfully!");
                return new SignInResponse(user.Id, true);
            } catch (Exception e) {
                _logger.LogCritical(2, $"{nameof(UserRepository)}.{nameof(SignIn)} Exception! {e.Message}");
                return new SignInResponse(null, false, new List<Error>() {new Error("NotSucceeded", $"Exception! {e.Message}")});
            }
        }
        public async Task<LogInResponse> CheckPassword(string username, string password) {
            try {
            AppUser user = await _userManager.FindByNameAsync(username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password)) {
                _logger.LogWarning(2, $"{nameof(UserRepository)}.{nameof(CheckPassword)} Invalid username {username} and password {password}!");
                return new LogInResponse(null, false, new List<Error>() {new Error("NotSucceeded", $"Invalid username or password!")});
            }
            return new LogInResponse(await FindUserByName(username), true);
            } catch (Exception e) {
                _logger.LogCritical(2, $"{nameof(UserRepository)}.{nameof(CheckPassword)} Exception! {e.Message}");
                return new LogInResponse(null, false, new List<Error>() {new Error("NotSucceeded", $"Exception! {e.Message}")});
            }
        }
        public async Task<PasswordResponse> ChangePassword(string id, string oldPassword, string newPassword)
        {
            try {
                AppUser appUser = await _userManager.FindByIdAsync(id);
                if (appUser == null)
                    return new PasswordResponse(null, false, new List<Error>(){new Error(null, "User not found!")});
                IdentityResult identityResult = await _userManager.ChangePasswordAsync(appUser, oldPassword, newPassword);
                if (identityResult.Succeeded)
                    return new PasswordResponse(appUser.Id, true, null);
                else {
                    _logger.LogError($"{nameof(ChangePassword)} failed to change password of user {id}!");
                    return new PasswordResponse(appUser.Id, false, identityResult.Errors.Select(e => new Error(e.Code, e.Description)).ToList());
                }
            } catch (Exception e) {
                _logger.LogError($"{nameof(ChangePassword)} exception", e);
                return null;
            }
        }
        private async Task<User> getUser(AppUser appUser) => appUser == null ? null : _mapper.Map<AppUser, User>(appUser, await GetSingleBySpec(new UserSpecification(appUser.Id)), opt => opt.ConfigureMap(MemberList.None));
        private async Task<FindUserResponse> getFindUserResponse(AppUser appUser)
        {
            User user = await getUser(appUser);
            return user == null ? new FindUserResponse(null, false, new List<Error>() { new Error(null, "User not found!") }) :
                new FindUserResponse(appUser.Id, true, null);
        }
    }
}