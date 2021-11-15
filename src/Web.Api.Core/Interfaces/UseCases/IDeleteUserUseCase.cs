using Web.Api.Core.DTO.UseCaseRequests;
namespace Web.Api.Core.Interfaces.UseCases;
public interface IDeleteUserUseCase : IUseCaseRequestHandler<DeleteUserRequest, UseCaseResponseMessage>
{
}