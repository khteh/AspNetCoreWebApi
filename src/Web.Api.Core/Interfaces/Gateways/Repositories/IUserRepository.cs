using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.DTO.GatewayResponses.Repositories;
namespace Web.Api.Core.Interfaces.Gateways.Repositories;
public interface IUserRepository : IRepository<User>
{
    Task<CreateUserResponse> Create(string firstName, string lastName, string email, string userName, string password);
    Task<DeleteUserResponse> Delete(string username);
    Task<FindUserResponse> FindById(string id);
    Task<User> FindUserByName(string userName);
    Task<FindUserResponse> FindByName(string userName);
    Task<FindUserResponse> FindByEmail(string email);
    Task<LogInResponse> CheckPassword(string username, string password);
    Task<PasswordResponse> ResetPassword(string id, string password);
    Task<PasswordResponse> ResetPassword(string email, string password, string code);
    Task<SignInResponse> SignIn(string username, string password, string remoteIP, bool rememberMe, bool logoutOnFailure);
    Task<SignInResponse> TwoFactorRecoveryCodeSignIn(string code);
    Task<SignInResponse> SignInWithClaims(string identityId, List<Claim> claims, AuthenticationProperties authProperties);
    Task<ExchangeRefreshTokenResponse> ExchangeRefreshToken(string accessToken, string refreshToken, string signingKey);
    Task<SignInResponse> SignInMobile(string username, string password, bool logoutOnFailure);
    Task<PasswordResponse> ChangePassword(string id, string oldPassword, string newPassword);
    Task<CodeResponse> RegistrationConfirmation(string email);
    Task<FindUserResponse> ConfirmEmail(string id, string code);
    Task<CodeResponse> GenerateChangeEmailToken(string identityId, string email);
    Task<FindUserResponse> ConfirmEmailChange(string identityId, string email, string code);
    Task<LockUserResponse> LockUser(string id);
    Task<LockUserResponse> UnLockUser(string id);
}