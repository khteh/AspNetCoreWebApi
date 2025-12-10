using AutoMapper;
using Web.Api.Core.DTO.UseCaseResponses;

namespace Web.Api.Presenters.Grpc;

public sealed class SignInPresenter : PresenterBase<LogInResponse>
{
    public Identity.Auth.LogInResponse Response { get; private set; }
    public SignInPresenter(IMapper mapper) : base(mapper) => Response = new Identity.Auth.LogInResponse() { Response = BaseResponse };
    public override async Task Handle(LogInResponse response)
    {
        await base.Handle(response);
        if (response.AccessToken != null)
            Response.AccessToken = new Identity.AccessToken()
            {
                Token = response.AccessToken.Token,
                ExpiresIn = response.AccessToken.ExpiresIn
            };
        if (response.RefreshToken != null)
            Response.RefreshToken = response.RefreshToken;
    }
}