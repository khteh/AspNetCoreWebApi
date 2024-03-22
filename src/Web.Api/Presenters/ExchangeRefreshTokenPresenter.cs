using System.Net;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Serialization;
namespace Web.Api.Presenters;
public sealed class ExchangeRefreshTokenPresenter : PresenterBase<ExchangeRefreshTokenResponse, Models.Response.ExchangeRefreshTokenResponse>
{
    public ExchangeRefreshTokenPresenter(ILogger<ExchangeRefreshTokenPresenter> logger) : base(logger) { }
    public override async Task Handle(ExchangeRefreshTokenResponse response)
    {
        Response = response.Success ? new Models.Response.ExchangeRefreshTokenResponse(response.AccessToken, response.RefreshToken, true, null) : new Models.Response.ExchangeRefreshTokenResponse(null, null, false, response.Errors);
        ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
        ContentResult.Content = JsonSerializer.SerializeObject(Response);
    }
}