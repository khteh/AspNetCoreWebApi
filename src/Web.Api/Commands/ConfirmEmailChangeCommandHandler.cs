using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces.UseCases;
using Web.Api.Models.Response;
using Web.Api.Presenters;
namespace Web.Api.Commands;
public class ConfirmEmailChangeCommandHandler : IRequestHandler<ConfirmEmailChangeCommand, ResponseBase>
{
    private readonly ILogger<ConfirmEmailChangeCommandHandler> _logger;
    private readonly IConfirmEmailChangeUseCase _useCase;
    private readonly SimplePresenter _presenter;
    public ConfirmEmailChangeCommandHandler(ILogger<ConfirmEmailChangeCommandHandler> logger, IConfirmEmailChangeUseCase useCase, SimplePresenter presenter)
    {
        _logger = logger;
        _useCase = useCase;
        _presenter = presenter;
    }
    public async Task<ResponseBase> Handle(ConfirmEmailChangeCommand command, CancellationToken cancellationToken)
    {
        await _useCase.Handle(new ConfirmEmailChangeRequest(command.IdentityId, command.Email, command.Code), _presenter);
        return _presenter.Response;
    }
}