using System.Net;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Serialization;
namespace Web.Api.Presenters;
public class GenerateNew2FARecoveryCodesPresenter : PresenterBase<Core.DTO.UseCaseResponses.GenerateNew2FARecoveryCodesResponse, Models.Response.GenerateNew2FARecoveryCodesResponse>
{
    public GenerateNew2FARecoveryCodesPresenter(ILogger<FindUserPresenter> logger) : base(logger) { }
    public override async Task Handle(Core.DTO.UseCaseResponses.GenerateNew2FARecoveryCodesResponse response)
    {
        ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
        ContentResult.Content = JsonSerializer.SerializeObject(new Models.Response.GenerateNew2FARecoveryCodesResponse(response.Id, response.Codes, response.Success, response.Errors));
    }
}