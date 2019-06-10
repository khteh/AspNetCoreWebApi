using System.Net;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Serialization;

namespace Web.Api.Presenters
{
    public sealed class LoginPresenter : PresenterBase<LoginResponse>
    {
        public override void Handle(LoginResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success ? JsonSerializer.SerializeObject(new Models.Response.LoginResponse(response.AccessToken, response.RefreshToken)) : JsonSerializer.SerializeObject(response.Errors);
        }
    }
}
