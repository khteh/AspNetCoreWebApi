using System.Threading.Tasks;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.UseCases;
namespace Web.Api.Core.UseCases;
public class ChangePasswordUseCase : IChangePasswordUseCase
{
    private readonly IUserRepository _userRepository;
    public ChangePasswordUseCase(IUserRepository userRepository) => _userRepository = userRepository;
    public async Task<bool> Handle(ChangePasswordRequest message, IOutputPort<UseCaseResponseMessage> outputPort)
    {
        var response = await _userRepository.ChangePassword(message.IdentityId, message.OldPassword, message.NewPassword);
        outputPort.Handle(response.Success ? new UseCaseResponseMessage(response.Id, true) : new UseCaseResponseMessage(response.Errors));
        return response.Success;
    }
}