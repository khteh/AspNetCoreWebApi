using System.Diagnostics.CodeAnalysis;
using Web.Api.Serialization;
namespace Web.Api.Presenters;

public class GenerateNew2FARecoveryCodesPresenter : PresenterBase<Core.DTO.UseCaseResponses.GenerateNew2FARecoveryCodesResponse, Models.Response.GenerateNew2FARecoveryCodesResponse>
{
    public GenerateNew2FARecoveryCodesPresenter(ILogger<GenerateNew2FARecoveryCodesPresenter> logger) : base(logger) { }
    public override async Task Handle(Core.DTO.UseCaseResponses.GenerateNew2FARecoveryCodesResponse response)
    {
        await base.Handle(response);
        Response?.Id = response.Id;
        Response?.Codes = response.Codes;
        if (Response != null)
            ContentResult.Content = JsonSerializer.SerializeObject(Response);
    }
}