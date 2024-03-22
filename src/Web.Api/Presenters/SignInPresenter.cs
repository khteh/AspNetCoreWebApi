using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;

namespace Web.Api.Presenters;

public class SignInPresenter : PresenterBase<UseCaseResponseMessage, SignInResponse>
{
    public SignInPresenter(ILogger<FindUserPresenter> logger) : base(logger) { }
}