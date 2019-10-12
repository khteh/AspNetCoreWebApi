using System;
using System.Threading.Tasks;
using Web.Api.Core.Auth;

namespace Web.Api.IntegrationTests.Services
{
    public interface IAuthGrpcClient<TMessage, TResponse> where TResponse : class
    {
        Task<LoginResponse> Login(LoginRequest request);
        Task<ExchangeRefreshTokenResponse> RefreshToken(ExchangeRefreshTokenRequest request);
    }
}