using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces.UseCases;
using Web.Api.Infrastructure.Auth;
using Web.Api.Models.Response;
using Web.Api.Presenters;

namespace Web.Api.Commands
{
    public class ExchangeRefreshTokenCommandHandler : IRequestHandler<ExchangeRefreshTokenCommand, ExchangeRefreshTokenResponse>
    {
        private readonly ILogger<ExchangeRefreshTokenCommandHandler> _logger;
        private readonly IExchangeRefreshTokenUseCase _useCase;
        private readonly ExchangeRefreshTokenPresenter _presenter;
        private readonly AuthSettings _authSettings;
        public ExchangeRefreshTokenCommandHandler(ILogger<ExchangeRefreshTokenCommandHandler> logger, IOptions<AuthSettings> authSettings, IExchangeRefreshTokenUseCase useCase, ExchangeRefreshTokenPresenter presenter)
        {
            _logger = logger;
            _useCase = useCase;
            _presenter = presenter;
            _authSettings = authSettings.Value;
        }
        public async Task<ExchangeRefreshTokenResponse> Handle(ExchangeRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            await _useCase.Handle(new ExchangeRefreshTokenRequest(request.AccessToken, request.RefreshToken, _authSettings.SecretKey), _presenter);
            return _presenter.Response;
        }
    }
}
