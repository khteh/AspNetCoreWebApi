using Web.Api.Serialization;
namespace Web.Api.Presenters;

public class GenerateNew2FARecoveryCodesPresenter : PresenterBase<Core.DTO.UseCaseResponses.GenerateNew2FARecoveryCodesResponse, Models.Response.GenerateNew2FARecoveryCodesResponse>
{
    public GenerateNew2FARecoveryCodesPresenter(ILogger<GenerateNew2FARecoveryCodesPresenter> logger) : base(logger) { }
    public override async Task Handle(Core.DTO.UseCaseResponses.GenerateNew2FARecoveryCodesResponse response)
    {
        await base.Handle(response);
        Response.Id = Guid.TryParse(response.Id, out Guid id) ? id.ToString() : string.Empty;
        Response.Codes = response.Codes;
        ContentResult.Content = JsonSerializer.SerializeObject(Response);
    }
}