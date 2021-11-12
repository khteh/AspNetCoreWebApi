using AutoMapper;
using Web.Api.Core.Interfaces;
namespace Web.Api.Presenters.Grpc;
public sealed class RegisterUserPresenter : PresenterBase<UseCaseResponseMessage>
{
    public Identity.Accounts.RegisterUserResponse Response {get; private set;}
    public RegisterUserPresenter(IMapper mapper) : base(mapper) {}
    public override void Handle(UseCaseResponseMessage response)
    {
        base.Handle(response);
        Response = new Identity.Accounts.RegisterUserResponse() { Id = response.Id, Response = BaseResponse };
    }
}