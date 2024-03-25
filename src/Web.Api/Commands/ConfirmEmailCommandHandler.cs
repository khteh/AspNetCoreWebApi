using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces.UseCases;
using Web.Api.Models.Response;
using Web.Api.Presenters;
namespace Web.Api.Commands;
public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, ResponseBase>
{
    private readonly ILogger<ConfirmEmailCommandHandler> _logger;
    private readonly IConfirmEmailUseCase _useCase;
    private readonly SimplePresenter _presenter;
    public ConfirmEmailCommandHandler(ILogger<ConfirmEmailCommandHandler> logger, IConfirmEmailUseCase useCase, SimplePresenter presenter)
    {
        _logger = logger;
        _useCase = useCase;
        _presenter = presenter;
    }
    public async Task<ResponseBase> Handle(ConfirmEmailCommand command, CancellationToken cancellationToken)
    {
        await _useCase.Handle(new ConfirmEmailRequest(command.IdentityId, command.Code), _presenter);
        return _presenter.Response;
    }
}