using System.Diagnostics.CodeAnalysis;
using Web.Api.Serialization;
namespace Web.Api.Presenters;

public class RegistrationConfirmationPresenter : PresenterBase<Core.DTO.UseCaseResponses.CodeResponse, Models.Response.CodeResponse>
{
    [SetsRequiredMembers]
    public RegistrationConfirmationPresenter(ILogger<RegistrationConfirmationPresenter> logger) : base(logger) { }
    public override async Task Handle(Core.DTO.UseCaseResponses.CodeResponse response)
    {
        await Handle(response, HttpStatusCode.Created, HttpStatusCode.BadRequest);
        Response.Id = response.Id;
        Response.Code = response.Code;
        ContentResult.Content = JsonSerializer.SerializeObject(Response);
    }
}