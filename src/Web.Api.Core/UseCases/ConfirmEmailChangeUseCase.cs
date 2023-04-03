using System.Threading.Tasks;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.UseCases;
namespace Web.Api.Core.UseCases;

public class ConfirmEmailChangeUseCase : IConfirmEmailChangeUseCase
{
    private readonly IUserRepository _userRepository;
    public ConfirmEmailChangeUseCase(IUserRepository userRepository) => _userRepository = userRepository;
    public async Task<bool> Handle(ConfirmEmailChangeRequest message, IOutputPort<UseCaseResponseMessage> outputPort)
    {
        DTO.GatewayResponses.Repositories.FindUserResponse response = await _userRepository.ConfirmEmailChange(message.IdentityId, message.Email, message.Code);
        await outputPort.Handle(response.Success ? new UseCaseResponseMessage(response.Id, true) : new UseCaseResponseMessage(response.Errors));
        return response.Success;
    }
}