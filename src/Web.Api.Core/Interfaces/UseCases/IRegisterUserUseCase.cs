using Web.Api.Core.DTO.UseCaseRequests;
namespace Web.Api.Core.Interfaces.UseCases;
public interface IRegisterUserUseCase : IUseCaseRequestHandler<RegisterUserRequest, UseCaseResponseMessage>
{
}