using System.Net;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
using Web.Api.Serialization;

namespace Web.Api.Presenters
{
    public sealed class RegisterUserPresenter : PresenterBase<UseCaseResponseMessage, RegisterUserResponse>
    {
        public override void Handle(UseCaseResponseMessage response)
        {
            Response = new Models.Response.RegisterUserResponse(response.Id, response.Success, response.Errors);
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.Created : HttpStatusCode.BadRequest);
            ContentResult.Content = JsonSerializer.SerializeObject(Response);
        }
    }
}