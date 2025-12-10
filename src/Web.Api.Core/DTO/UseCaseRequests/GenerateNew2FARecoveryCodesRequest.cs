using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
namespace Web.Api.Core.DTO.UseCaseRequests;

public class GenerateNew2FARecoveryCodesRequest : IUseCaseRequest<GenerateNew2FARecoveryCodesResponse>
{
    public string Id { get; }
    public int Codes { get; }
    public GenerateNew2FARecoveryCodesRequest(string id, int codes) => (Id, Codes) = (id, codes);
}