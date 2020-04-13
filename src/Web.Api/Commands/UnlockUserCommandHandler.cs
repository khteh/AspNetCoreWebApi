using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Api.Core.Interfaces.UseCases;
using Web.Api.Models.Response;
using Web.Api.Presenters;

namespace Web.Api.Commands
{
    public class UnlockUserCommandHandler : IRequestHandler<UnlockUserCommand, LockUserResponse>
    {
        private readonly ILogger<UnlockUserCommandHandler> _logger;
        private readonly ILockUserUseCase _useCase;
        private readonly LockUserPresenter _presenter;
        public UnlockUserCommandHandler(ILogger<UnlockUserCommandHandler> logger, ILockUserUseCase useCase, LockUserPresenter presenter)
        {
            _logger = logger;
            _useCase = useCase;
            _presenter = presenter;
        }
        public async Task<LockUserResponse> Handle(UnlockUserCommand request, CancellationToken cancellationToken)
        {
            bool response = await _useCase.UnLock(request.Id, _presenter);
            return _presenter.Response;
        }
    }
}
