using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces.UseCases;
using Web.Api.Models.Response;
using Web.Api.Presenters;
namespace Web.Api.Commands;
public class GenerateChangeEmailTokenCommandHandler : IRequestHandler<GenerateChangeEmailTokenCommand, CodeResponse>
{
    private readonly ILogger<GenerateChangeEmailTokenCommandHandler> _logger;
    private readonly IGenerateChangeEmailTokenUseCase _useCase;
    private readonly CodePresenter _presenter;
    public GenerateChangeEmailTokenCommandHandler(ILogger<GenerateChangeEmailTokenCommandHandler> logger, IGenerateChangeEmailTokenUseCase useCase, CodePresenter presenter)
    {
        _logger = logger;
        _useCase = useCase;
        _presenter = presenter;
    }
    public async Task<CodeResponse> Handle(GenerateChangeEmailTokenCommand command, CancellationToken cancellationToken)
    {
        bool response = await _useCase.Handle(new GenerateChangeEmailTokenRequest(command.IdentityId, command.Email), _presenter);
        return _presenter.Response;
    }
}