using System.Threading.Tasks;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.UseCases;
namespace Web.Api.Core.UseCases;
public class DeleteUserUseCase : IDeleteUserUseCase
{
    private readonly IUserRepository _userRepository;
    public DeleteUserUseCase(IUserRepository userRepository) => _userRepository = userRepository;
    public async Task<bool> Handle(DeleteUserRequest message, IOutputPort<UseCaseResponseMessage> outputPort)
    {
        var response = await _userRepository.Delete(message.UserName);
        await outputPort.Handle(response.Success ? new UseCaseResponseMessage(response.Id.ToString(), true) : new UseCaseResponseMessage(response.Errors));
        return response.Success;
    }
}