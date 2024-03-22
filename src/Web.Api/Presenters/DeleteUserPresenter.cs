using System.Net;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
namespace Web.Api.Presenters;
public class DeleteUserPresenter : PresenterBase<UseCaseResponseMessage, DeleteUserResponse>
{
    public DeleteUserPresenter(ILogger<DeleteUserPresenter> logger) : base(logger) { }
    public override async Task Handle(UseCaseResponseMessage response)
    {
        await base.Handle(response);
        ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.NoContent : HttpStatusCode.BadRequest);
    }
}