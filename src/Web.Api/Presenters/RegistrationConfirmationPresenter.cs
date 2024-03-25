using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
namespace Web.Api.Presenters;
public class RegistrationConfirmationPresenter : PresenterBase<Core.DTO.UseCaseResponses.CodeResponse, ChangePasswordResponse>
{
    public RegistrationConfirmationPresenter(ILogger<FindUserPresenter> logger) : base(logger) { }
}