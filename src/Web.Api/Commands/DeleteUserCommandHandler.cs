using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces.UseCases;
using Web.Api.Models.Response;
using Web.Api.Presenters;
namespace Web.Api.Commands;
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, DeleteUserResponse>
{
    private readonly ILogger<DeleteUserCommandHandler> _logger;
    private readonly IDeleteUserUseCase _useCase;
    private readonly DeleteUserPresenter _presenter;
    public DeleteUserCommandHandler(ILogger<DeleteUserCommandHandler> logger, IDeleteUserUseCase useCase, DeleteUserPresenter presenter)
    {
        _logger = logger;
        _useCase = useCase;
        _presenter = presenter;
    }
    public async Task<DeleteUserResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        await _useCase.Handle(new DeleteUserRequest(request.UserName), _presenter);
        return _presenter.Response;
    }
}