using System;
using System.Net;
using Web.Api.Core.Interfaces;
using Web.Api.Serialization;

namespace Web.Api.Presenters.Grpc
{
    public abstract class PresenterBase<T, TResponse> : IOutputPort<T> where T : UseCaseResponseMessage
    {
        public TResponse Response {get; set;}
        public virtual void Handle(T response) =>
            Response = (TResponse)Activator.CreateInstance(typeof(TResponse), new object[] { response.Success, response.Errors });
    }
}