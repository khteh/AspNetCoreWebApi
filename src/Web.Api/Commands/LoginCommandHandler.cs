using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces.UseCases;
using Web.Api.Models.Response;
using Web.Api.Presenters;
namespace Web.Api.Commands;
public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly ILogger<LoginCommandHandler> _logger;
    private readonly ILoginUseCase _useCase;
    private readonly LoginPresenter _presenter;
    public LoginCommandHandler(ILogger<LoginCommandHandler> logger, ILoginUseCase useCase, LoginPresenter presenter)
    {
        _logger = logger;
        _useCase = useCase;
        _presenter = presenter;
    }
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        await _useCase.Handle(new LoginRequest(request.UserName, request.Password, request.RemoteIpAddress), _presenter);
        return _presenter.Response;
    }
}