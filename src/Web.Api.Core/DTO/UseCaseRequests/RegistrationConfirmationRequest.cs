using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
namespace Web.Api.Core.DTO.UseCaseRequests;
public class RegistrationConfirmationRequest : IUseCaseRequest<CodeResponse>
{
    public string Email { get; }
    public RegistrationConfirmationRequest(string email) => Email = email;
}