using System;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Web.Api.Core.Configuration;
using Web.Api.IntegrationTests.Accounts;
using Web.Api.IntegrationTests.Grpc;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading.Tasks;
using Web.Api.Core.Grpc;
using Response = Web.Api.IntegrationTests.Grpc.Response;
using static Web.Api.IntegrationTests.Accounts.Accounts;

namespace Web.Api.IntegrationTests.Services
{
    class AccountsGrpcClient<TMessage, TResponse> : IAccountsGrpcClient<TMessage, TResponse> where TResponse : class
    {
        private readonly ILogger<AccountsGrpcClient<TMessage, TResponse>> _logger;
        private readonly AccountsClient _client;
        private readonly GrpcConfig _config;
        public AccountsGrpcClient(ILogger<AccountsGrpcClient<TMessage, TResponse>> logger, IOptions<GrpcConfig> options, HttpClient httpClient)
        {
            _logger = logger;
            _config = options.Value;
            GrpcChannel channel = GrpcChannel.ForAddress(_config.Endpoint, new GrpcChannelOptions
                {
                    HttpClient = httpClient//CreateHttpClient(true)
                });
            _client = new AccountsClient(channel);
        }
        public async Task<RegisterUserResponse> Register(RegisterUserRequest request) => await _client.RegisterAsync(request);
        public async Task<Response> ChangePassword(ChangePasswordRequest request) => await _client.ChangePasswordAsync(request);
        public async Task<Response> ResetPassword(ResetPasswordRequest request) => await _client.ResetPasswordAsync(request);
        public async Task<Response> Lock(StringInputParameter request) => await _client.LockAsync(request);
        public async Task<Response> UnLock(StringInputParameter request) => await _client.UnLockAsync(request);
        public async Task<DeleteUserResponse> Delete(StringInputParameter request) => await _client.DeleteAsync(request);
        public async Task<FindUserResponse> FindById (StringInputParameter request) => await _client.FindByIdAsync(request);
        public async Task<FindUserResponse> FindByUserName (StringInputParameter request) => await _client.FindByUserNameAsync(request);
        public async Task<FindUserResponse> FindByEmail (StringInputParameter request) => await _client.FindByEmailAsync(request);
    }
}