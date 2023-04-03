using System;
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
}
public abstract class PresenterBase<T, TResponse> : IOutputPort<T> where T : UseCaseResponseMessage where TResponse : ResponseBase
{
    public TResponse Response { get; protected set; }
    public JsonContentResult ContentResult { get; } = new JsonContentResult();
    public virtual async Task Handle(T response)
    {
        Response = (TResponse)Activator.CreateInstance(typeof(TResponse), new object[] { response.Success, response.Errors });
        ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
        ContentResult.Content = JsonSerializer.SerializeObject(Response);
    }
}