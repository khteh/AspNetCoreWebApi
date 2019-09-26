using System.Net;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Serialization;

namespace Web.Api.Presenters.Grpc
{
    public sealed class RegisterUserPresenter : PresenterBase<UseCaseResponseMessage, Web.Api.Core.Grpc.Response>
    {
        public override void Handle(UseCaseResponseMessage response) => base.Handle(response);
    }
}