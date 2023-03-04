using Web.Api.Core.DTO.UseCaseRequests;
namespace Web.Api.Core.Interfaces.UseCases;

public interface IConfirmEmailChangeUseCase : IUseCaseRequestHandler<ConfirmEmailChangeRequest, UseCaseResponseMessage>
{
}