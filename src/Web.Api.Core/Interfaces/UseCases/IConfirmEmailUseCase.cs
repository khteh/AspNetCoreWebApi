using Web.Api.Core.DTO.UseCaseRequests;
namespace Web.Api.Core.Interfaces.UseCases;
public interface IConfirmEmailUseCase : IUseCaseRequestHandler<ConfirmEmailRequest, UseCaseResponseMessage>
{
}