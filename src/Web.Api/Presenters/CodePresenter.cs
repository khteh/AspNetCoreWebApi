using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
namespace Web.Api.Presenters;
public class CodePresenter : PresenterBase<Core.DTO.UseCaseResponses.CodeResponse, CodeResponse>
{
    public CodePresenter(ILogger<CodePresenter> logger) : base(logger) { }

    public override async Task Handle(Core.DTO.UseCaseResponses.CodeResponse response)
    {
        await base.Handle(response);
        Response.Code = response.Code;
    }
}