using System.Linq;
using System.Threading.Tasks;
using Web.Api.Core.Dto.UseCaseRequests;
using Web.Api.Core.Dto.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.UseCases;

namespace Web.Api.Core.UseCases
{
    public class DeleteUserUseCase : IDeleteUserUseCase
    {
        private readonly IUserRepository _userRepository;
        public DeleteUserUseCase(IUserRepository userRepository) => _userRepository = userRepository;
        public async Task<bool> Handle(DeleteUserRequest message, IOutputPort<DeleteUserResponse> outputPort)
        {
            var response = await _userRepository.Delete(message.UserName);
            outputPort.Handle(response.Success ? new DeleteUserResponse(response.Id.ToString(), true) : new DeleteUserResponse(response.Errors));
            return response.Success;
        }
    }
}