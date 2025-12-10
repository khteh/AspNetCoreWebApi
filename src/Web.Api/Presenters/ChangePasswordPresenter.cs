using System.Diagnostics.CodeAnalysis;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
namespace Web.Api.Presenters;

public class ChangePasswordPresenter : PresenterBase<UseCaseResponseMessage, ChangePasswordResponse>
{
    public ChangePasswordPresenter(ILogger<ChangePasswordPresenter> logger) : base(logger) { }
}