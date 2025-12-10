using System.Diagnostics.CodeAnalysis;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
namespace Web.Api.Presenters;
public class ResetPasswordPresenter : PresenterBase<UseCaseResponseMessage, ResetPasswordResponse>
{
    [SetsRequiredMembers]
    public ResetPasswordPresenter(ILogger<ResetPasswordPresenter> logger) : base(logger) { }
}