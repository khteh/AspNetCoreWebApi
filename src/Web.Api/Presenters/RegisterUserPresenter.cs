using System.Net;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Serialization;

namespace Web.Api.Presenters
{
    public sealed class RegisterUserPresenter : PresenterBase<RegisterUserResponse>
    {
        public override void Handle(RegisterUserResponse response)
        {
            base.Handle(response);
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.Created : HttpStatusCode.BadRequest);
        }
    }
}