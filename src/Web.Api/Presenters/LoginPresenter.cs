using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Serialization;
namespace Web.Api.Presenters;
public sealed class LoginPresenter : PresenterBase<LoginResponse, Models.Response.LogInResponse>
{
    public override void Handle(LoginResponse response)
    {
        Response = response.Success ? new Models.Response.LogInResponse(response.AccessToken, response.RefreshToken, true, null) : new Models.Response.LogInResponse(null, null, false, response.Errors);
        ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
        ContentResult.Content = JsonSerializer.SerializeObject(Response);
    }
}