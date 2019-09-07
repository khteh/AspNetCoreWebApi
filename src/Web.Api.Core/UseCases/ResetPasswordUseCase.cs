using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.UseCases;

namespace Web.Api.Core.UseCases
{
    public class ResetPasswordUseCase : IResetPasswordUseCase
    {
        private readonly IUserRepository _userRepository;
        public ResetPasswordUseCase(IUserRepository userRepository) => _userRepository = userRepository;
        public async Task<bool> Handle(ResetPasswordRequest request, IOutputPort<UseCaseResponseMessage> outputPort)
        {
            DTO.GatewayResponses.Repositories.PasswordResponse response = await _userRepository.ResetPassword(request.Id, request.NewPassword);
            if (response == null)
            {
                outputPort.Handle(new UseCaseResponseMessage(null, false, $"Failed to reset password of user {request.Id}", new List<Error>() { new Error(HttpStatusCode.InternalServerError.ToString(), $"Failed to reset password of user {request.Id}")}));
                return false;
            } else
                outputPort.Handle(response.Success ? new UseCaseResponseMessage(request.Id, true, null) : new UseCaseResponseMessage(response.Errors));
            return response.Success;
        }
    }
}