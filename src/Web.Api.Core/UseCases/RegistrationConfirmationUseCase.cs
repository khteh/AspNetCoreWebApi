using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Helpers;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.UseCases;
namespace Web.Api.Core.UseCases;
public sealed class RegistrationConfirmationUseCase : IRegistrationConfirmationUseCase
{
    private readonly ILogger<RegistrationConfirmationUseCase> _logger;
    private readonly IUserRepository _userRepository;
    public RegistrationConfirmationUseCase(ILogger<RegistrationConfirmationUseCase> logger, IUserRepository userRepository) => (_logger, _userRepository) = (logger, userRepository);
    public async Task<bool> Handle(RegistrationConfirmationRequest message, IOutputPort<CodeResponse> outputPort)
    {
        DTO.GatewayResponses.Repositories.CodeResponse response = await _userRepository.RegistrationConfirmation(message.Email);
        if (response.Success)
            _logger.LogInformation($"{nameof(RegistrationConfirmationUseCase)} Successfully confirm user registration of Id: {response.Id}, ConfirmationCode: {response.Code}");
        else
        {
            StringBuilder sb = new StringBuilder();
            foreach (DTO.Error error in response.Errors)
                sb.Append($"{error.Code} {error.Description}");
            _logger.LogError($"{nameof(RegistrationConfirmationUseCase)} Failed to confirm user registration of Id: {response.Id}, ConfirmationCode: {response.Code}, reasons: reasons: {sb.ToString()}");
        }
        string errMsg = response.Errors != null && response.Errors.Any() ? response.Errors.First().Description : string.Empty;
        await outputPort.Handle(new CodeResponse(response.Id, response.Code, response.Success, errMsg, response.Errors));
        return response.Success;
    }
}