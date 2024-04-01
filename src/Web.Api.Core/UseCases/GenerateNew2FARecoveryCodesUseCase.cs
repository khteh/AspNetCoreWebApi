using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Helpers;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.UseCases;
namespace Web.Api.Core.UseCases;
public class GenerateNew2FARecoveryCodesUseCase : IGenerateNew2FARecoveryCodesUseCase
{
    private readonly IUserRepository _userRepository;
    public GenerateNew2FARecoveryCodesUseCase(IUserRepository userRepository) => _userRepository = userRepository;
    public async Task<bool> Handle(GenerateNew2FARecoveryCodesRequest message, IOutputPort<GenerateNew2FARecoveryCodesResponse> outputPort)
    {
        DTO.GatewayResponses.Repositories.GenerateNew2FARecoveryCodesResponse response = await _userRepository.GenerateNew2FARecoveryCodes(message.Id, message.Codes);
        string errMsg = response.Errors != null && response.Errors.Any() ? response.Errors.First().Description : string.Empty;
        await outputPort.Handle(new GenerateNew2FARecoveryCodesResponse(response.Id, response.Codes, response.Success, errMsg, response.Errors));
        return response.Success;
    }
}