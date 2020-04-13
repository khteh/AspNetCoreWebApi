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
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ChangePasswordResponse>
    {
        private readonly ILogger<ChangePasswordCommandHandler> _logger;
        private readonly IChangePasswordUseCase _useCase;
        private readonly ChangePasswordPresenter _presenter;
        public ChangePasswordCommandHandler(ILogger<ChangePasswordCommandHandler> logger, IChangePasswordUseCase useCase, ChangePasswordPresenter presenter)
        {
            _logger = logger;
            _useCase = useCase;
            _presenter = presenter;
        }
        public async Task<ChangePasswordResponse> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            await _useCase.Handle(new ChangePasswordRequest(request.IdentityId, request.OldPassword, request.NewPassword), _presenter);
            return _presenter.Response;
        }
    }
}