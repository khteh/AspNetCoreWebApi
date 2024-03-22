using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using Web.Api.Core.DTO.UseCaseRequests;
using Web.Api.Core.Interfaces.UseCases;
using Web.Api.Infrastructure.Auth;
using Web.Api.Models.Response;
using Web.Api.Presenters;
namespace Web.Api.Commands;
public class GenerateNew2FARecoveryCodesCommandHandler : IRequestHandler<GenerateNew2FARecoveryCodesCommand, GenerateNew2FARecoveryCodesResponse>
{
    private readonly ILogger<GenerateNew2FARecoveryCodesCommandHandler> _logger;
    private readonly IGenerateNew2FARecoveryCodesUseCase _useCase;
    private readonly GenerateNew2FARecoveryCodesPresenter _presenter;
    public GenerateNew2FARecoveryCodesCommandHandler(ILogger<GenerateNew2FARecoveryCodesCommandHandler> logger, IGenerateNew2FARecoveryCodesUseCase useCase, GenerateNew2FARecoveryCodesPresenter presenter)
    {
        _logger = logger;
        _useCase = useCase;
        _presenter = presenter;
    }
    public async Task<GenerateNew2FARecoveryCodesResponse> Handle(GenerateNew2FARecoveryCodesCommand command, CancellationToken cancellationToken)
    {
        bool response = await _useCase.Handle(new GenerateNew2FARecoveryCodesRequest(command.Id, command.Codes), _presenter);
        return _presenter.Response;
    }
}