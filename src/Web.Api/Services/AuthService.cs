using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Web.Api.Core.Auth;

namespace Web.Api.Services
{
    public class AuthService : Auth.AuthBase
    {
        private readonly ILogger<AuthService> _logger;
        public AuthService(ILogger<AuthService> logger) => _logger = logger;

    }
}