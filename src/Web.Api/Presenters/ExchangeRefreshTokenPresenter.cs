using System.Diagnostics.CodeAnalysis;
using System.Net;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Serialization;
namespace Web.Api.Presenters;
public sealed class ExchangeRefreshTokenPresenter : PresenterBase<ExchangeRefreshTokenResponse, Models.Response.ExchangeRefreshTokenResponse>
{
    [SetsRequiredMembers]
    public ExchangeRefreshTokenPresenter(ILogger<ExchangeRefreshTokenPresenter> logger) : base(logger) { }
    public override async Task Handle(ExchangeRefreshTokenResponse response)
    {
        await base.Handle(response);
        Response.AccessToken = response.AccessToken;
        Response.RefreshToken = response.RefreshToken;
        ContentResult.Content = JsonSerializer.SerializeObject(Response);
    }
}