using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.UseCases;
using Web.Api.Presenters.Grpc;
using Web.Api.Identity.Accounts;
using Web.Api.Identity.Auth;
using Web.Api.Infrastructure.Auth;
using System;

namespace Web.Api.Services
{
    public class AuthService : Auth.AuthBase
    {
        private readonly ILogger<AuthService> _logger;
        private readonly ILoginUseCase _loginUseCase;
        private readonly LoginPresenter _loginPresenter;
        private readonly IExchangeRefreshTokenUseCase _exchangeRefreshTokenUseCase;
        private readonly ExchangeRefreshTokenPresenter _exchangeRefreshTokenPresenter;
        private readonly AuthSettings _authSettings;
        public AuthService(ILogger<AuthService> logger, ILoginUseCase loginUseCase, LoginPresenter loginPresenter, IExchangeRefreshTokenUseCase exchangeRefreshTokenUseCase, ExchangeRefreshTokenPresenter exchangeRefreshTokenPresenter, IOptions<AuthSettings> authSettings)
        {
            _logger = logger;
            _loginUseCase = loginUseCase;
            _loginPresenter = loginPresenter;
            _exchangeRefreshTokenUseCase = exchangeRefreshTokenUseCase;
            _exchangeRefreshTokenPresenter = exchangeRefreshTokenPresenter;
            _authSettings = authSettings.Value;
        }

        public async override Task<Web.Api.Identity.Auth.LoginResponse> Login(Web.Api.Identity.Auth.LoginRequest request, ServerCallContext context)
        {
            await _loginUseCase.Handle(new Web.Api.Core.DTO.UseCaseRequests.LoginRequest(request.UserName, request.Password, context.GetHttpContext().Request.HttpContext.Connection.RemoteIpAddress?.ToString()), _loginPresenter);
            return _loginPresenter.Response;
        }

        // POST api/auth/refreshtoken
        public async override Task<Web.Api.Identity.Auth.ExchangeRefreshTokenResponse> RefreshToken(Web.Api.Identity.Auth.ExchangeRefreshTokenRequest request, ServerCallContext context)
        {
            await _exchangeRefreshTokenUseCase.Handle(new Web.Api.Core.DTO.UseCaseRequests.ExchangeRefreshTokenRequest(request.AccessToken, request.RefreshToken, _authSettings.SecretKey), _exchangeRefreshTokenPresenter);
            return _exchangeRefreshTokenPresenter.Response;
        }

    }
}