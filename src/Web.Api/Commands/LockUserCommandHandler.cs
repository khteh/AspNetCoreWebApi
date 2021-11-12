using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Web.Api.Core.Interfaces.UseCases;
using Web.Api.Models.Response;
using Web.Api.Presenters;
namespace Web.Api.Commands;
public class LockUserCommandHandler : IRequestHandler<LockUserCommand, LockUserResponse>
{
    private readonly ILogger<LockUserCommandHandler> _logger;
    private readonly ILockUserUseCase _useCase;
    private readonly LockUserPresenter _presenter;
    public LockUserCommandHandler(ILogger<LockUserCommandHandler> logger, ILockUserUseCase useCase, LockUserPresenter presenter)
    {
        _logger = logger;
        _useCase = useCase;
        _presenter = presenter;
    }
    public async Task<LockUserResponse> Handle(LockUserCommand request, CancellationToken cancellationToken)
    {
        bool response = await _useCase.Lock(request.Id, _presenter);
        return _presenter.Response;
    }
}