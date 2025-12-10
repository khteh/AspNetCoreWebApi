using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web.Api.Core.DTO;
using Web.Api.Core.DTO.GatewayResponses.Repositories;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Interfaces.Services;
using Web.Api.Core.Interfaces.UseCases;

namespace Web.Api.Core.UseCases;

public sealed class LogInUseCase : ILogInUseCase
{
    private readonly ILogger<LogInUseCase> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IJwtFactory _jwtFactory;
    private readonly ITokenFactory _tokenFactory;
    public LogInUseCase(ILogger<LogInUseCase> logger, IUserRepository userRepository, IJwtFactory jwtFactory, ITokenFactory tokenFactory)
    {
        _logger = logger;
        _userRepository = userRepository;
        _jwtFactory = jwtFactory;
        _tokenFactory = tokenFactory;
    }
    public async Task<bool> Handle(LogInRequest message, IOutputPort<DTO.UseCaseResponses.LogInResponse> outputPort)
    {
        LogInResponse? result = null;
        if (!string.IsNullOrEmpty(message.UserName) && !string.IsNullOrEmpty(message.Password))
        {
            result = await _userRepository.CheckPassword(message.UserName, message.Password);
            if (result != null && result.Success && result.User != null)
            {
                // generate refresh token
                var refreshToken = _tokenFactory.GenerateToken();
                result.User.AddRefreshToken(refreshToken, message.RemoteIpAddress);
                await _userRepository.Update(result.User);
                // generate access token
                await outputPort.Handle(new DTO.UseCaseResponses.LogInResponse(await _jwtFactory.GenerateEncodedToken(result.User.IdentityId, result.User.UserName), refreshToken, true));
                return true;
            }
            else if (result != null && result.Errors != null && result.Errors.Any())
            {
                foreach (Error e in result.Errors)
                    _logger.LogError($"{nameof(LogInUseCase)} error: {e.Code} {e.Description}");
            }
        }
        await outputPort.Handle(new DTO.UseCaseResponses.LogInResponse(result != null && result.Errors != null && result.Errors.Any() ? result.Errors : new List<Error>() { new Error(HttpStatusCode.BadRequest.ToString(), "Invalid username or password!") }));
        return false;
    }
}