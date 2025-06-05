using Web.Api.Serialization;
namespace Web.Api.Presenters;

public class RegistrationConfirmationPresenter : PresenterBase<Core.DTO.UseCaseResponses.CodeResponse, Web.Api.Models.Response.CodeResponse>
{
    public RegistrationConfirmationPresenter(ILogger<RegistrationConfirmationPresenter> logger) : base(logger) { }
    public override async Task Handle(Core.DTO.UseCaseResponses.CodeResponse response)
    {
        await Handle(response, HttpStatusCode.Created, HttpStatusCode.BadRequest);
        Response.Id = Guid.TryParse(response.Id, out Guid id) ? id : Guid.Empty;
        Response.Code = response.Code;
        ContentResult.Content = JsonSerializer.SerializeObject(Response);
    }
}