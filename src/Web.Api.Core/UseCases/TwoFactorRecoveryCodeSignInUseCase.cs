using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.UseCases;

namespace Web.Api.Core.UseCases;

public class TwoFactorRecoveryCodeSignInUseCase : ITwoFactorRecoveryCodeSignInUseCase
{
    private readonly IUserRepository _userRepository;
    public TwoFactorRecoveryCodeSignInUseCase(IUserRepository repo) => _userRepository = repo;
    public async Task<bool> Handle(TwoFactorRecoveryCodeSignInRequest message, IOutputPort<SignInResponse> outputPort)
    {
        DTO.GatewayResponses.Repositories.SignInResponse? result = null;
        if (!string.IsNullOrEmpty(message.Code))
        {
            result = await _userRepository.TwoFactorRecoveryCodeSignIn(message.Code);
            if (result != null && result.Success && result.UserId != Guid.Empty)
            {
                await outputPort.Handle(new SignInResponse(result.UserId, result.UserName!, true, "Signed in successfully!"));
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