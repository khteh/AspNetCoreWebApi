using System.Diagnostics.CodeAnalysis;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Serialization;
namespace Web.Api.Presenters;

public class SignInPresenter : PresenterBase<SignInResponse, Models.Response.SignInResponse>
{
    public SignInPresenter(ILogger<SignInPresenter> logger) : base(logger) { }
    public override async Task Handle(SignInResponse response) =>
        await Handle(response, HttpStatusCode.OK, HttpStatusCode.Unauthorized);
}