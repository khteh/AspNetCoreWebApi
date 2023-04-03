using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.UseCases;
namespace Web.Api.Core.UseCases;
public class ConfirmEmailUseCase : IConfirmEmailUseCase
{
    private readonly ILogger<ConfirmEmailUseCase> _logger;
    private readonly IUserRepository _userRepository;
    public ConfirmEmailUseCase(ILogger<ConfirmEmailUseCase> logger, IUserRepository userRepository) => (_logger, _userRepository) = (logger, userRepository);
    public async Task<bool> Handle(ConfirmEmailRequest message, IOutputPort<UseCaseResponseMessage> outputPort)
    {
        Web.Api.Core.DTO.GatewayResponses.Repositories.FindUserResponse response = await _userRepository.ConfirmEmail(message.IdentityId, message.Code);
        if (response.Success)
            _logger.LogInformation($"{nameof(ConfirmEmailUseCase)} Successfully confirm email of user Id: {message.IdentityId}, Code: {message.Code}");
        else
        {
            StringBuilder sb = new StringBuilder();
            foreach (DTO.Error error in response.Errors)
                sb.Append($"{error.Code} {error.Description}");
            _logger.LogError($"{nameof(ConfirmEmailUseCase)} Failed to confirm email of user id: {message.IdentityId}, code: {message.Code}, reasons: {sb.ToString()}");
        }
        await outputPort.Handle(response.Success ? new UseCaseResponseMessage(response.Id, true) : new UseCaseResponseMessage(response.Errors));
        return response.Success;
    }
}