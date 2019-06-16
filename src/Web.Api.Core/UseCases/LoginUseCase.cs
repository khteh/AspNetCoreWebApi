using System.Threading.Tasks;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.GatewayResponses.Repositories;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.Services;
using Web.Api.Core.Interfaces.UseCases;

namespace Web.Api.Core.UseCases
{
    public sealed class LoginUseCase : ILoginUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtFactory _jwtFactory;
        private readonly ITokenFactory _tokenFactory;

        public LoginUseCase(IUserRepository userRepository, IJwtFactory jwtFactory, ITokenFactory tokenFactory)
        {
            _userRepository = userRepository;
            _jwtFactory = jwtFactory;
            _tokenFactory = tokenFactory;
        }

        public async Task<bool> Handle(LoginRequest message, IOutputPort<LoginResponse> outputPort)
        {
            LogInResponse result = null;
            if (!string.IsNullOrEmpty(message.UserName) && !string.IsNullOrEmpty(message.Password))
            {
                result = await _userRepository.CheckPassword(message.UserName, message.Password);
                if (result != null && result.Success && result.User != null) {
                    // generate refresh token
                    var refreshToken = _tokenFactory.GenerateToken();
                    result.User.AddRefreshToken(refreshToken, message.RemoteIpAddress);
                    await _userRepository.Update(result.User);
                    // generate access token
                    outputPort.Handle(new LoginResponse(await _jwtFactory.GenerateEncodedToken(result.User.IdentityId, result.User.UserName), refreshToken, true));
                    return true;
                }
            }
            outputPort.Handle(new LoginResponse(result != null ? result.Errors : new System.Collections.Generic.List<Error>() { new Error("login_failure", "Invalid username or password.") }));
            return false;
        }
    }
}