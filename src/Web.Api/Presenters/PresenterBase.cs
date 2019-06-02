using System.Net;
using Web.Api.Core.Interfaces;
using Web.Api.Serialization;

namespace Web.Api.Presenters
{
    public abstract class PresenterBase<T> : IOutputPort<T> where T : UseCaseResponseMessage
    {
        public JsonContentResult ContentResult { get; } = new JsonContentResult();
        public virtual void Handle(T response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            ContentResult.Content = response.Success ? JsonSerializer.SerializeObject(response) : JsonSerializer.SerializeObject(response.Errors);
        }
    }
}