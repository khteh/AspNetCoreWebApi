using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces.UseCases;
using Web.Api.Models.Response;
using Web.Api.Presenters;
namespace Web.Api.Commands;
public class SignInCommandHandler : IRequestHandler<SignInCommand, SignInResponse>
{
    private readonly ILogger<SignInCommandHandler> _logger;
    private readonly ISignInUseCase _useCase;
    private readonly SignInPresenter _presenter;
    public SignInCommandHandler(ILogger<SignInCommandHandler> logger, ISignInUseCase useCase, SignInPresenter presenter)
    {
        _logger = logger;
        _useCase = useCase;
        _presenter = presenter;
    }
    public async Task<SignInResponse> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        // public SignInRequest(string username, string password, bool isPersistent = false, bool lockoutOnFailure = true,  bool isMobile = false)
        await _useCase.Handle(new SignInRequest(request.UserName, request.Password, request.RemoteIpAddress, request.rememberMe, request.lockOutOnFailure, request.isMobile), _presenter);
        return _presenter.Response;
    }
}