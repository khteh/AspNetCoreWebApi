using System;
using System.Linq;
using System.Net;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
using Web.Api.Serialization;
namespace Web.Api.Presenters;
public abstract class PresenterBase<T> : IOutputPort<T> where T : UseCaseResponseMessage
{
    public JsonContentResult ContentResult { get; } = new JsonContentResult();
    public virtual async Task Handle(T response)
    {
        ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
        ContentResult.Content = JsonSerializer.SerializeObject(response);
    }
    public virtual async Task Handle(T response, HttpStatusCode success = HttpStatusCode.OK, HttpStatusCode failure = HttpStatusCode.BadRequest)
    {
        await Handle(response);
        ContentResult.StatusCode = (int)(response.Success ? success : failure);
    }
}
public abstract class PresenterBase<T, TResponse> : IOutputPort<T> where T : UseCaseResponseMessage where TResponse : ResponseBase
{
    public TResponse Response { get; set; }
    public JsonContentResult ContentResult { get; } = new JsonContentResult();
    protected readonly Microsoft.Extensions.Logging.ILogger _logger;
    public PresenterBase(Microsoft.Extensions.Logging.ILogger logger) => _logger = logger;
    public virtual async Task Handle(T response) => await BuildResponse(response);
    protected async Task Handle(T response, HttpStatusCode success = HttpStatusCode.OK, HttpStatusCode failure = HttpStatusCode.BadRequest)
    {
        await BuildResponse(response);
        ContentResult.StatusCode = (int)(response.Success ? success : failure);
    }
    private async Task BuildResponse(T response)
    {
        if (!response.Success)
            _logger.LogError($"{nameof(PresenterBase<T, TResponse>)} operation failed! {response.Errors?.FirstOrDefault().Description}");
        else
            _logger.LogDebug($"{nameof(PresenterBase<T, TResponse>)} operation succeeded!");
        Response = (TResponse)Activator.CreateInstance(typeof(TResponse), new object[] { response.Success, response.Errors });
        ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
        ContentResult.Content = JsonSerializer.SerializeObject(Response);
    }
}