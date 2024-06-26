using AutoMapper;
using Web.Api.Core.DTO.UseCaseResponses;
namespace Web.Api.Presenters.Grpc;
public class RegistrationConfirmationPresenter : PresenterBase<CodeResponse>
{
    public Identity.Accounts.CodeResponse Response { get; private set; }
    public RegistrationConfirmationPresenter(IMapper mapper) : base(mapper) { }
    public override async Task Handle(CodeResponse response)
    {
        await base.Handle(response);
        Response = new Identity.Accounts.CodeResponse() { Code = response.Code };
    }
}