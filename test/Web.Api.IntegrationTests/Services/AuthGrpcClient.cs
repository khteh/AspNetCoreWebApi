using System;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Web.Api.Core.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading.Tasks;
using static Web.Api.IntegrationTests.Auth.Auth;
using Web.Api.IntegrationTests.Auth;

namespace Web.Api.IntegrationTests.Services
{
    class AuthGrpcClient<TMessage, TResponse> : IAuthGrpcClient<TMessage, TResponse> where TResponse : class
    {
        private readonly ILogger<AuthGrpcClient<TMessage, TResponse>> _logger;
        private readonly AuthClient _client;
        private readonly GrpcConfig _config;
        public AuthGrpcClient(ILogger<AuthGrpcClient<TMessage, TResponse>> logger, IOptions<GrpcConfig> options, HttpClient httpClient)
        {
            _logger = logger;
            _config = options.Value;
            GrpcChannel channel = GrpcChannel.ForAddress(_config.Endpoint, new GrpcChannelOptions
            {
                HttpClient = httpClient
            });
            _client = new AuthClient(channel);
        }
        public async Task<LoginResponse> Login(LoginRequest request) => await _client.LoginAsync(request);
        public async Task<ExchangeRefreshTokenResponse> RefreshToken(ExchangeRefreshTokenRequest request) => await _client.RefreshTokenAsync(request);
    }
}