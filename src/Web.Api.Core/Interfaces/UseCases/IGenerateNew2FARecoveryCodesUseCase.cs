using System.Threading.Tasks;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.DTO.UseCaseResponses;
namespace Web.Api.Core.Interfaces.UseCases;

public interface IGenerateNew2FARecoveryCodesUseCase : IUseCaseRequestHandler<GenerateNew2FARecoveryCodesRequest, GenerateNew2FARecoveryCodesResponse>
{
}