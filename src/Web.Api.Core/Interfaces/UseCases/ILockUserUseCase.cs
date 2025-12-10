using System;
using System.Threading.Tasks;
using Web.Api.Core.DTO.UseCaseRequests;
namespace Web.Api.Core.Interfaces.UseCases;

public interface ILockUserUseCase : IUseCaseRequestHandler<LockUserRequest, UseCaseResponseMessage>
{
    Task<bool> Lock(Guid id, IOutputPort<UseCaseResponseMessage> outputPort);
    Task<bool> UnLock(Guid id, IOutputPort<UseCaseResponseMessage> outputPort);
}