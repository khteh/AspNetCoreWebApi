using System.Net;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
namespace Web.Api.Presenters;
public class DeleteUserPresenter : PresenterBase<UseCaseResponseMessage, DeleteUserResponse>
{
    public override void Handle(UseCaseResponseMessage response)
    {
        base.Handle(response);
        ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.NoContent : HttpStatusCode.BadRequest);
    }
}