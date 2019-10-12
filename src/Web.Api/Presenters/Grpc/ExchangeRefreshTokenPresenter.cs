using System.Net;
using System.Linq;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Serialization;
using Web.Api.Core.DTO;
using AutoMapper;
namespace Web.Api.Presenters.Grpc
{
    public sealed class ExchangeRefreshTokenPresenter : PresenterBase<ExchangeRefreshTokenResponse>
    {
        public Web.Api.Core.Auth.ExchangeRefreshTokenResponse Response {get; private set;}
        public ExchangeRefreshTokenPresenter(IMapper mapper) : base(mapper) {}
        public override void Handle(ExchangeRefreshTokenResponse response)
        {
            base.Handle(response);
            Response = new Web.Api.Core.Auth.ExchangeRefreshTokenResponse() { Response = BaseResponse };
            if (response.AccessToken != null)
                Response.AccessToken = new Web.Api.Core.Grpc.AccessToken() {
                        Token = response.AccessToken.Token,
                        ExpiresIn = response.AccessToken.ExpiresIn
                    };
            if (response.RefreshToken != null)
                Response.RefreshToken = response.RefreshToken;
        }
    }
}