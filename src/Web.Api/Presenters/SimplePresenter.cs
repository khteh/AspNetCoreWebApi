using System.Diagnostics.CodeAnalysis;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
namespace Web.Api.Presenters;

public class SimplePresenter : PresenterBase<UseCaseResponseMessage, ResponseBase>
{
    public SimplePresenter(ILogger<SimplePresenter> logger) : base(logger) { }
}