using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.UseCases;

namespace Web.Api.Core.UseCases;

public class SignInUseCase : ISignInUseCase
{
    private readonly IUserRepository _userRepository;
    public SignInUseCase(IUserRepository repo) => _userRepository = repo;
    public async Task<bool> Handle(SignInRequest message, IOutputPort<SignInResponse> outputPort)
    {
        DTO.GatewayResponses.Repositories.SignInResponse result = null;
        if (!string.IsNullOrEmpty(message.UserName) && !string.IsNullOrEmpty(message.Password))
        {
            result = message.IsMobileApp ? await _userRepository.SignInMobile(message.UserName, message.Password, message.LockOutOnFailure)
                                            : await _userRepository.SignIn(message.UserName, message.Password, message.RemoteIpAddress, message.RememberMe, message.LockOutOnFailure);
            if (result != null && result.Success && result.UserId != Guid.Empty)
            {
                // public SignInResponse(Guid id, string username, bool success = false, string message = null)
                await outputPort.Handle(new SignInResponse(result.UserId, result.UserName, true, "Signed in successfully!"));
                return true;
            }
        }
        if (result != null)
            await outputPort.Handle(new SignInResponse(result.RequiresTwoFactor, result.IsLockedOut, result.Errors));
        else
            await outputPort.Handle(new SignInResponse(false, false, new List<DTO.Error>() { new DTO.Error("login_failure", "Invalid username or password.") }));
        return false;
    }
}