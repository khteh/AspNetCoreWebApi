using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces.UseCases;
using Web.Api.Models.Response;
using Web.Api.Presenters;

namespace Web.Api.Commands
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordResponse>
    {
        private readonly ILogger<ResetPasswordCommandHandler> _logger;
        private readonly IResetPasswordUseCase _useCase;
        private readonly ResetPasswordPresenter _presenter;
        public ResetPasswordCommandHandler(ILogger<ResetPasswordCommandHandler> logger, IResetPasswordUseCase useCase, ResetPasswordPresenter presenter)
        {
            _logger = logger;
            _useCase = useCase;
            _presenter = presenter;
        }
        public async Task<ResetPasswordResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            await _useCase.Handle(new ResetPasswordRequest(request.Id, request.NewPassword), _presenter);
            return _presenter.Response;
        }
    }
}