using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.UseCases;

namespace Web.Api.Core.UseCases;

public class SignInWithClaimsUseCase : ISignInWithClaimsUseCase
{
    private readonly IUserRepository _userRepository;
    public SignInWithClaimsUseCase(IUserRepository repo) => _userRepository = repo;
    public async Task<bool> Handle(SignInWithClaimsRequest message, IOutputPort<SignInResponse> outputPort)
    {
        DTO.GatewayResponses.Repositories.SignInResponse? result = null;
        if (!string.IsNullOrEmpty(message.IdentityId))
        {
            result = await _userRepository.SignInWithClaims(message.IdentityId, message.Claims, message.AuthProperties);
            if (result != null && result.Success && result.UserId != Guid.Empty && !string.IsNullOrEmpty(result.UserName))
            {
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