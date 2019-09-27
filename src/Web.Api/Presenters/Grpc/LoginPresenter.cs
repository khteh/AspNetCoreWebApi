using System.Net;
using System.Linq;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Serialization;
using Web.Api.Core.DTO;
namespace Web.Api.Presenters.Grpc
{
    public sealed class LoginPresenter : IOutputPort<LoginResponse>
    {
        public Web.Api.Core.Auth.LoginResponse Response {get; private set;}
        public void Handle(LoginResponse response)
        {
            Response = new Web.Api.Core.Auth.LoginResponse();
            if (response.AccessToken != null)
                Response.AccessToken = new Web.Api.Core.Grpc.AccessToken() {
                    Token = response.AccessToken.Token,
                    ExpiresIn = response.AccessToken.ExpiresIn
                };
            if (response.RefreshToken != null)
                Response.RefreshToken = response.RefreshToken;
            Response.Response = new Web.Api.Core.Grpc.Response();
            Response.Response.Success = response.Success;
            if (response.Errors != null && response.Errors.Any())
                foreach (Error error in response.Errors)
                    Response.Response.Errors.Add(new Web.Api.Core.Grpc.Error() {Code = error.Code, Description = error.Description});
        }
    }
}