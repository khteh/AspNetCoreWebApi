using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
namespace Web.Api.Core.DTO.UseCaseRequests;
public class ForgotPasswordRequest : IUseCaseRequest<CodeResponse>
{
    public string Email { get; }
    public ForgotPasswordRequest(string email) => Email = email;
}