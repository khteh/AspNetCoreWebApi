using System.Net;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
using Web.Api.Serialization;
namespace Web.Api.Presenters;
public sealed class RegisterUserPresenter : PresenterBase<UseCaseResponseMessage, RegisterUserResponse>
{
    public RegisterUserPresenter(ILogger<RegisterUserPresenter> logger) : base(logger) { }
    public override async Task Handle(UseCaseResponseMessage response)
    {
        Response = new RegisterUserResponse(response.Id, response.Success, response.Errors);
        ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.Created : HttpStatusCode.BadRequest);
        ContentResult.Content = JsonSerializer.SerializeObject(Response);
    }
}