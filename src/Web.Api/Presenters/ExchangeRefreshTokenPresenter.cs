using System.Net;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Serialization;

namespace Web.Api.Presenters
{
    public sealed class ExchangeRefreshTokenPresenter : PresenterBase<ExchangeRefreshTokenResponse, Models.Response.ExchangeRefreshTokenResponse>
    {
        public override void Handle(ExchangeRefreshTokenResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            ContentResult.Content = response.Success ? JsonSerializer.SerializeObject(new Models.Response.ExchangeRefreshTokenResponse(response.AccessToken, response.RefreshToken, true, null)) : 
                                                        JsonSerializer.SerializeObject(new Models.Response.ExchangeRefreshTokenResponse(null, null, false, response.Errors));
        }
    }
}