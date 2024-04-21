using Web.Api.Serialization;
namespace Web.Api.Presenters;
public class RegistrationConfirmationPresenter : PresenterBase<Core.DTO.UseCaseResponses.CodeResponse, Web.Api.Models.Response.CodeResponse>
{
    private readonly ILogger<RegistrationConfirmationPresenter> _logger;
    public RegistrationConfirmationPresenter(ILogger<RegistrationConfirmationPresenter> logger) : base(logger) => _logger = logger;
    public override async Task Handle(Core.DTO.UseCaseResponses.CodeResponse response)
    {
        await Handle(response, HttpStatusCode.Created, HttpStatusCode.BadRequest);
        Response.Id = Guid.Parse(response.Id);
        Response.Code = response.Code;
        ContentResult.Content = JsonSerializer.SerializeObject(Response);
    }
}