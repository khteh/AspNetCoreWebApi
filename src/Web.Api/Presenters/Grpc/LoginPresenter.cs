using System.Net;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Serialization;

namespace Web.Api.Presenters.Grpc
{
    public sealed class LoginPresenter : PresenterBase<LoginResponse, Web.Api.Core.Auth.LoginResponse>
    {
        public override void Handle(LoginResponse response) =>
            Response = new Web.Api.Core.Auth.LoginResponse()
            {
                UserId = response.Id,
                AccessToken = response.AccessToken,
                RefreshToken = response.RefreshToken,
                Response = new Web.Api.Grpc.Response()
                {
                    Success = response.Success,
                    Errors = response.Errors
                }
            };
    }
}