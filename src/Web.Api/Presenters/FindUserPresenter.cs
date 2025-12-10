using System.Diagnostics.CodeAnalysis;
using System.Net;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Serialization;
namespace Web.Api.Presenters;

public class FindUserPresenter : PresenterBase<FindUserResponse, Models.Response.FindUserResponse>
{
    public FindUserPresenter(ILogger<FindUserPresenter> logger) : base(logger) { }
    public override async Task Handle(FindUserResponse response)
    {
        await base.Handle(response);
        Response.Id = response.Id;
        Response.User = response.User;
        ContentResult.Content = JsonSerializer.SerializeObject(Response);
    }
}