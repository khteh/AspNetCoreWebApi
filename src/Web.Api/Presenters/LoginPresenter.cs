using System.Diagnostics.CodeAnalysis;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Serialization;
namespace Web.Api.Presenters;
public sealed class LogInPresenter : PresenterBase<LogInResponse, Models.Response.LogInResponse>
{
    [SetsRequiredMembers]
    public LogInPresenter(ILogger<LogInPresenter> logger) : base(logger) { }
    public override async Task Handle(LogInResponse response)
    {
        await Handle(response, HttpStatusCode.OK, HttpStatusCode.Unauthorized);
        Response.AccessToken = response.AccessToken;
        Response.RefreshToken = response.RefreshToken;
        ContentResult.Content = JsonSerializer.SerializeObject(Response);
    }
}