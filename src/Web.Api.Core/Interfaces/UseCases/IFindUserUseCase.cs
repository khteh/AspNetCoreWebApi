using System.Threading.Tasks;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.DTO.UseCaseResponses;
namespace Web.Api.Core.Interfaces.UseCases;
public interface IFindUserUseCase : IUseCaseRequestHandler<FindUserRequest, FindUserResponse>
{
    Task<bool> FindByEmail(string normalizedEmail, IOutputPort<FindUserResponse> outputPort);
    Task<bool> FindById(string userId, IOutputPort<FindUserResponse> outputPort);
    Task<bool> FindByName(string normalizedUserName, IOutputPort<FindUserResponse> outputPort);
}