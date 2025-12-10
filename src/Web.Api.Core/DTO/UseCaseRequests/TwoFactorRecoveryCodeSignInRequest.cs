using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;

namespace Web.Api.Core.DTO.UseCaseRequests;

public class TwoFactorRecoveryCodeSignInRequest : IUseCaseRequest<SignInResponse>
{
    public required string Code { get; set; }
    public TwoFactorRecoveryCodeSignInRequest(string code) => Code = code;
}