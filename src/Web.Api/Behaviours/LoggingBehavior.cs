using Web.Api.Extensions;
namespace Web.Api.Behaviours;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"----- Handling command {request.GetGenericTypeName()} ({request})");
        string line = $"----- Handling command {request.GetGenericTypeName()} ({request})";
        if (line.ToLower().Contains("password"))
        {
            int start = line.ToLower().IndexOf("password = ");

            // eg { password = dummyPassword, ... }
            int end = line.ToLower().IndexOf(',', start);
            if (end < 0)
            {
                // check if password is the last param
                // eg { ..., password = dummyPassword }
                end = line.ToLower().IndexOf(' ', start + "password = ".Length);
            }
            string substr = line.Substring(start, end - start);
            line = line.Replace(substr, new string('*', end - start));
        }
        _logger.LogInformation(line);
        var response = await next();
        _logger.LogInformation($"----- Command {request.GetGenericTypeName()} handled - response: {response}");
        return response;
    }
}