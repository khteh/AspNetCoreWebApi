using System.Threading.Tasks;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.UseCases;
namespace Web.Api.Core.UseCases;
public sealed class RegisterUserUseCase : IRegisterUserUseCase
{
    private readonly IUserRepository _userRepository;
    public RegisterUserUseCase(IUserRepository userRepository) => _userRepository = userRepository;
    public async Task<bool> Handle(RegisterUserRequest message, IOutputPort<UseCaseResponseMessage> outputPort)
    {
        var response = await _userRepository.Create(message.FirstName, message.LastName, message.Email, message.UserName, message.Password);
        outputPort.Handle(response.Success ? new UseCaseResponseMessage(response.Id, true) : new UseCaseResponseMessage(response.Errors));
        return response.Success;
    }
}