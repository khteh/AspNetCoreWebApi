using System;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Web.Api.Core.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading.Tasks;

namespace Web.Api.IntegrationTests.Services
{
    class AuthClient<TMessage, TResponse> : GrpcClientBase<TMessage, TResponse> where TResponse : class
    {
        private readonly ILogger<AuthClient<TMessage, TResponse>> _logger;
        private readonly Accounts.Accounts.AccountsClient _client;
        public AuthClient(ILogger<AuthClient<TMessage, TResponse>> logger, IOptions<GrpcConfig> options, HttpClient httpClient) : base(options, httpClient)
        {
            _logger = logger;
            _client = new Accounts.Accounts.AccountsClient(_channel);
        }
        public async override Task<TResponse> Handle(object message)
        {
            if (message is HelloRequest) {
                GrpcGreeter.HelloReply response = await _client.SayHelloAsync(message as HelloRequest);
                _logger.LogInformation(response.Message);
            } else
                _logger.LogError($"{nameof(AuthClient<TMessage, TResponse>)}.{nameof(Handle)} Invalid message type! {message.GetType().Name}");
        }
    }
}