using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces.UseCases;
using Web.Api.Models.Response;
using Web.Api.Presenters;
namespace Web.Api.Commands;
public class SignInCommandHandler : IRequestHandler<SignInCommand, LogInResponse>
{
    private readonly ILogger<SignInCommandHandler> _logger;
    private readonly ILoginUseCase _useCase;
    private readonly LoginPresenter _presenter;
    public SignInCommandHandler(ILogger<SignInCommandHandler> logger, ILoginUseCase useCase, LoginPresenter presenter)
    {
        _logger = logger;
        _useCase = useCase;
        _presenter = presenter;
    }
    public async Task<LogInResponse> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        await _useCase.Handle(new LoginRequest(request.UserName, request.Password, request.RemoteIpAddress), _presenter);
        return _presenter.Response;
    }
}