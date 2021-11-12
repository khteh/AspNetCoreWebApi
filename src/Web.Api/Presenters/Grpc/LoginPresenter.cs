using AutoMapper;
using Web.Api.Core.DTO.UseCaseResponses;
namespace Web.Api.Presenters.Grpc;
public sealed class LoginPresenter : PresenterBase<LoginResponse>
{
    public Web.Api.Identity.Auth.LoginResponse Response {get; private set;}
    public LoginPresenter(IMapper mapper) : base(mapper) {}
    public override void Handle(LoginResponse response)
    {
        base.Handle(response);
        Response = new Web.Api.Identity.Auth.LoginResponse() { Response = BaseResponse };
        if (response.AccessToken != null)
            Response.AccessToken = new Web.Api.Identity.AccessToken() {
                Token = response.AccessToken.Token,
                ExpiresIn = response.AccessToken.ExpiresIn
            };
        if (response.RefreshToken != null)
            Response.RefreshToken = response.RefreshToken;
    }
}