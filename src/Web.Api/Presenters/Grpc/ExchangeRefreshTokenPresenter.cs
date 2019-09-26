using System.Net;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Serialization;

namespace Web.Api.Presenters.Grpc
{
    public sealed class ExchangeRefreshTokenPresenter : PresenterBase<ExchangeRefreshTokenResponse, Web.Api.Core.Grpc.Response.ExchangeRefreshTokenResponse>
    {
        public override void Handle(ExchangeRefreshTokenResponse response) =>
            Response = new Web.Api.Core.Grpc.ExchangeRefreshTokenResponse()
            {
                AccessToken = response.AccessToken,
                RefreshToken = response.RefreshToken,
                Response = new Web.Api.Core.Grpc.Response()
                {
                    Success = response.Success,
                    Errors = response.Errors
                }
            };
    }
}