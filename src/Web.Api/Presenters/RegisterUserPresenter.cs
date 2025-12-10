using System.Diagnostics.CodeAnalysis;
using System.Net;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
using Web.Api.Serialization;
namespace Web.Api.Presenters;

public sealed class RegisterUserPresenter : PresenterBase<UseCaseResponseMessage, RegisterUserResponse>
{
    public RegisterUserPresenter(ILogger<RegisterUserPresenter> logger) : base(logger) { }
    public override async Task Handle(UseCaseResponseMessage response)
    {
        await Handle(response, HttpStatusCode.Created, HttpStatusCode.BadRequest);
        Response.Id = response.Id.ToString();
        ContentResult.Content = JsonSerializer.SerializeObject(Response);
    }
}