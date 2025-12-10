using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.UseCases;
namespace Web.Api.Core.UseCases;

public sealed class LockUserUseCase : ILockUserUseCase
{
    private readonly IUserRepository _userRepository;
    public LockUserUseCase(IUserRepository userRepository) => _userRepository = userRepository;
    public async Task<bool> Handle(LockUserRequest message, Interfaces.IOutputPort<UseCaseResponseMessage> outputPort) =>
            throw new NotImplementedException($"Please use Lock or Unlock interface!");
    public async Task<bool> Lock(Guid id, Interfaces.IOutputPort<UseCaseResponseMessage> outputPort)
    {
        DTO.GatewayResponses.Repositories.LockUserResponse response = await _userRepository.LockUser(id);
        if (response == null)
        {
            await outputPort.Handle(new UseCaseResponseMessage(Guid.Empty, false, $"Failed to lock user {id}", new List<Error> { new Error(HttpStatusCode.InternalServerError.ToString(), $"Failed to lock user {id}") }));
            return false;
        }
        else
            await outputPort.Handle(response.Success ? new UseCaseResponseMessage(id, true, null) : new UseCaseResponseMessage(response.Errors!));
        return response.Success;
    }
    public async Task<bool> UnLock(Guid id, Interfaces.IOutputPort<UseCaseResponseMessage> outputPort)
    {
        DTO.GatewayResponses.Repositories.LockUserResponse response = await _userRepository.UnLockUser(id);
        if (response == null)
        {
            await outputPort.Handle(new UseCaseResponseMessage(Guid.Empty, false, $"Failed to unlock user {id}", new List<Error> { new Error(HttpStatusCode.InternalServerError.ToString(), $"Failed to unlock user {id}") }));
            return false;
        }
        else
            await outputPort.Handle(response.Success ? new UseCaseResponseMessage(id, true, null) : new UseCaseResponseMessage(response.Errors!));
        return response.Success;
    }
}