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
        /*
        Refresh token rotation means replacing the token each time it's used, not cycling through a fixed set. 
        It's the same sense as "rotating" a password or an API key: you retire the current one and issue a new one.

        each call to /api/auth/refreshtoken does this:

        1. It checks the presented refresh token against the stored hash.
        2. It deletes that token (RemoveRefreshToken).
        3. It generates a brand-new random one (GenerateToken, 32 fresh bytes) and stores its hash (AddRefreshToken).
        4. It returns the new one with the new access token.

        So each refresh token works exactly once. That's why the OAuth 2.0 security guidance calls this "refresh token rotation", and why the two terms "rotated" and "single-use" describe the same thing.

        Why it matters for security:
        Without rotation, a refresh token is valid for its full 5 days however many times it's used, so a stolen one gives an attacker 5 days of access. With rotation:

        1. The window shrinks. A stolen token stops working as soon as the legitimate client uses it.
        2. Theft becomes detectable. If the attacker and the real client both hold the same token, whoever uses it second presents a token that has already been exchanged. That should never happen in normal use, so your new reuse check treats it as theft and revokes all of that user's refresh tokens. Both parties are then logged out, and the real user just logs in again.        
        */
        if (string.IsNullOrEmpty(message.AccessToken) || string.IsNullOrEmpty(message.RefreshToken) || string.IsNullOrEmpty(message.SigningKey))
        {
            await outputPort.Handle(new ExchangeRefreshTokenResponse(new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), "Access token and refresh token are required.") }));
            return false;
        }
        DTO.GatewayResponses.Repositories.ExchangeRefreshTokenResponse response = await _userRepository.ExchangeRefreshToken(message.AccessToken, message.RefreshToken, message.SigningKey);
        await outputPort.Handle(response.Success && response.AccessToken != null && response.RefreshToken != null ? new ExchangeRefreshTokenResponse(response.AccessToken, response.RefreshToken, true) : new ExchangeRefreshTokenResponse(response.Errors));
        return response.Success;
    }
}