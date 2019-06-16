using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.UseCases;

namespace Web.Api.Core.UseCases
{
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
                                            : await _userRepository.SignIn(message.UserName, message.Password, message.RememberMe, message.LockOutOnFailure);
                if (result != null && result.Success && !string.IsNullOrEmpty(result.UserId)) {
                    outputPort.Handle(new SignInResponse(result.UserId, true, "Signed in successfully!", null));
                    return true;
                }
            }
            outputPort.Handle(new SignInResponse(result != null ? result.Errors : new List<Error>() { new Error("login_failure", "Invalid username or password.") }));
            return false;
        }
    }
}