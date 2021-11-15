using System.Threading.Tasks;
using Web.Api.Core.DTO.UseCaseRequests;
namespace Web.Api.Core.Interfaces.UseCases;
public interface ILockUserUseCase : IUseCaseRequestHandler<LockUserRequest, UseCaseResponseMessage>
{
    Task<bool> Lock(string id, IOutputPort<UseCaseResponseMessage> outputPort);
    Task<bool> UnLock(string id, IOutputPort<UseCaseResponseMessage> outputPort);
}