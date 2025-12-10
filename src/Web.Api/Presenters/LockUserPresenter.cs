using System.Diagnostics.CodeAnalysis;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
namespace Web.Api.Presenters;
public class LockUserPresenter : PresenterBase<UseCaseResponseMessage, LockUserResponse>
{
    [SetsRequiredMembers]
    public LockUserPresenter(ILogger<LockUserPresenter> logger) : base(logger) { }
}