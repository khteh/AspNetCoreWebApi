using Web.Api.Extensions;
namespace Web.Api.Behaviours;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Never log request / response objects directly: record ToString() includes passwords, tokens and keys.
        _logger.LogInformation("----- Handling command {CommandName} ({Command})", request.GetGenericTypeName(), LogSanitizer.Describe(request));
        var response = await next();
        _logger.LogInformation("----- Command {CommandName} handled - response: {Response}", request.GetGenericTypeName(), LogSanitizer.Describe(response));
        return response;
    }
}
