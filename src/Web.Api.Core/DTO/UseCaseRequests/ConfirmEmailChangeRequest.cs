using Web.Api.Core.Interfaces;
namespace Web.Api.Core.DTO.UseCaseRequests;

public class ConfirmEmailChangeRequest : IUseCaseRequest<UseCaseResponseMessage>
{
    public string IdentityId { get; set; }
    public string Code { get; set; }
    public string Email { get; set; }
    public ConfirmEmailChangeRequest(string id, string email, string code) => (IdentityId, Email, Code) = (id, email, code);
}