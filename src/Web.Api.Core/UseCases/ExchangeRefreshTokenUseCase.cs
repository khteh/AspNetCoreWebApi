using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Web.Api.Core.DTO;
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
        if (string.IsNullOrEmpty(message.AccessToken) || string.IsNullOrEmpty(message.RefreshToken) || string.IsNullOrEmpty(message.SigningKey))
        {
            await outputPort.Handle(new ExchangeRefreshTokenResponse(new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), $"Invalid access token {message.AccessToken} / refresh token {message.RefreshToken} / signing key {message.SigningKey}!") }));
            return false;
        }
        DTO.GatewayResponses.Repositories.ExchangeRefreshTokenResponse response = await _userRepository.ExchangeRefreshToken(message.AccessToken, message.RefreshToken, message.SigningKey);
        await outputPort.Handle(response.Success ? new ExchangeRefreshTokenResponse(response.AccessToken, response.RefreshToken, true) : new ExchangeRefreshTokenResponse(response.Errors));
        return response.Success;
    }
}