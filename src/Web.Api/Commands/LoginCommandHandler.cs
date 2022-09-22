using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces.UseCases;
using Web.Api.Models.Response;
using Web.Api.Presenters;
namespace Web.Api.Commands;
public class LogInCommandHandler : IRequestHandler<LogInCommand, LogInResponse>
{
    private readonly ILogger<LogInCommandHandler> _logger;
    private readonly ILogInUseCase _useCase;
    private readonly LogInPresenter _presenter;
    public LogInCommandHandler(ILogger<LogInCommandHandler> logger, ILogInUseCase useCase, LogInPresenter presenter)
    {
        _logger = logger;
        _useCase = useCase;
        _presenter = presenter;
    }
    public async Task<LogInResponse> Handle(LogInCommand request, CancellationToken cancellationToken)
    {
        await _useCase.Handle(new LogInRequest(request.UserName, request.Password, request.RemoteIpAddress), _presenter);
        return _presenter.Response;
    }
}