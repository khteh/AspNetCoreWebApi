using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
namespace Web.Api.Presenters;
public class LockUserPresenter : PresenterBase<UseCaseResponseMessage, LockUserResponse>
{
    public LockUserPresenter(ILogger<LockUserPresenter> logger) : base(logger) { }
}