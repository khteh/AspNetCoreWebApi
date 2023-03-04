using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
namespace Web.Api.Core.DTO.UseCaseRequests;

public class GenerateChangeEmailTokenRequest : IUseCaseRequest<CodeResponse>
{
    public string IdentityId { get; }
    public string Email { get; }
    public GenerateChangeEmailTokenRequest(string identityId, string email) => (IdentityId, Email) = (identityId, email);
}