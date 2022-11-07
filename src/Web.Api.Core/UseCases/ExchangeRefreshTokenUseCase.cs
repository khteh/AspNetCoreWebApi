using System.Threading.Tasks;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.UseCases;
namespace Web.Api.Core.UseCases;
public sealed class ExchangeRefreshTokenUseCase : IExchangeRefreshTokenUseCase
{
    private readonly IUserRepository _userRepository;
    public ExchangeRefreshTokenUseCase(IUserRepository userRepository) => _userRepository = userRepository;
    public async Task<bool> Handle(ExchangeRefreshTokenRequest message, IOutputPort<ExchangeRefreshTokenResponse> outputPort)
    {
        DTO.GatewayResponses.Repositories.ExchangeRefreshTokenResponse response = await _userRepository.ExchangeRefreshToken(message.AccessToken, message.RefreshToken, message.SigningKey);
        outputPort.Handle(response.Success ? new ExchangeRefreshTokenResponse(response.AccessToken, response.RefreshToken, true) : new ExchangeRefreshTokenResponse(response.Errors));
        return response.Success;
    }
}