using AutoMapper;
using Web.Api.Core.DTO.UseCaseResponses;
namespace Web.Api.Presenters.Grpc;
public sealed class ExchangeRefreshTokenPresenter : PresenterBase<ExchangeRefreshTokenResponse>
{
    public Web.Api.Identity.Auth.ExchangeRefreshTokenResponse Response { get; private set; }
    public ExchangeRefreshTokenPresenter(IMapper mapper) : base(mapper) { }
    public override async Task Handle(ExchangeRefreshTokenResponse response)
    {
        await base.Handle(response);
        Response = new Web.Api.Identity.Auth.ExchangeRefreshTokenResponse() { Response = BaseResponse };
        if (response.AccessToken != null)
            Response.AccessToken = new Web.Api.Identity.AccessToken()
            {
                Token = response.AccessToken.Token,
                ExpiresIn = response.AccessToken.ExpiresIn
            };
        if (response.RefreshToken != null)
            Response.RefreshToken = response.RefreshToken;
    }
}