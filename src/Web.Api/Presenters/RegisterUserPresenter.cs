using System.Net;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Serialization;

namespace Web.Api.Presenters
{
    public sealed class RegisterUserPresenter : PresenterBase<UseCaseResponseMessage>
    {
        public override void Handle(UseCaseResponseMessage response)
        {
            base.Handle(response);
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.Created : HttpStatusCode.BadRequest);
        }
    }
}