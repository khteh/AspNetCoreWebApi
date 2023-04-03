using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.UseCases;
namespace Web.Api.Core.UseCases;
public class ResetPasswordUseCase : IResetPasswordUseCase
{
    private ILogger<ResetPasswordUseCase> _logger;
    private readonly IUserRepository _userRepository;
    public ResetPasswordUseCase(ILogger<ResetPasswordUseCase> logger, IUserRepository userRepository) => (_logger, _userRepository) = (logger, userRepository);
    public async Task<bool> Handle(ResetPasswordRequest request, IOutputPort<UseCaseResponseMessage> outputPort)
    {
        DTO.GatewayResponses.Repositories.PasswordResponse response = null;
        if (!string.IsNullOrEmpty(request.Id))
            response = await _userRepository.ResetPassword(request.Id, request.NewPassword);
        else if (!string.IsNullOrEmpty(request.Email) && !string.IsNullOrEmpty(request.Code))
            response = await _userRepository.ResetPassword(request.Email, request.NewPassword, request.Code);
        else
        {
            await outputPort.Handle(new UseCaseResponseMessage(string.Empty, false, $"Failed to reset password of user {request.Id}", new List<DTO.Error>() { new DTO.Error(HttpStatusCode.BadRequest.ToString(), $"Invalid input parameters to reset password!") }));
            return false;
        }
        if (response == null)
        {
            await outputPort.Handle(new UseCaseResponseMessage(string.Empty, false, $"Failed to reset password of user {request.Id}", new List<DTO.Error>() { new DTO.Error(HttpStatusCode.InternalServerError.ToString(), $"Failed to reset password of user {request.Id}") }));
            return false;
        }
        else
        {
            if (response.Success)
                _logger.LogInformation($"User {request.Email} reset password successfully!");
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (DTO.Error error in response.Errors)
                    sb.Append($"{error.Code} {error.Description}");
                _logger.LogError($"User {request.Email} failed to reset password! {sb.ToString()}");
            }
            await outputPort.Handle(response.Success ? new UseCaseResponseMessage(request.Id, true, null) : new UseCaseResponseMessage(response.Errors));
        }
        return response.Success;
    }
}