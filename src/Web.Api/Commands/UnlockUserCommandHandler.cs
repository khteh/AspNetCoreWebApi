using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Web.Api.Core.Interfaces;
using Web.Api.Core.Interfaces.UseCases;
using Web.Api.Models.Response;
using Web.Api.Presenters;
namespace Web.Api.Commands;

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
        if (Guid.TryParse(request.Id, out Guid id))
        {
            await _useCase.UnLock(id, _presenter);
            return _presenter.Response;
        }
        else
        {
            await _presenter.Handle(new UseCaseResponseMessage(Guid.Empty, false, $"Failed to unlock user! Invalid id {request.Id}!", new List<Core.DTO.Error>() { new Core.DTO.Error("Failed to unlock user!", $"Invalid id {request.Id}!") }));
            return _presenter.Response;
        }
    }
}