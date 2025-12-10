using System.Diagnostics.CodeAnalysis;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
using Web.Api.Serialization;
namespace Web.Api.Presenters;

public class CodePresenter : PresenterBase<Core.DTO.UseCaseResponses.CodeResponse, CodeResponse>
{
    public CodePresenter(ILogger<CodePresenter> logger) : base(logger) { }
    public override async Task Handle(Core.DTO.UseCaseResponses.CodeResponse response)
    {
        await base.Handle(response);
        Response.Id = response.Id;
        Response.Code = response.Code;
        ContentResult.Content = JsonSerializer.SerializeObject(Response);
    }
}