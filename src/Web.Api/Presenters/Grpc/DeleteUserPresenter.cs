using AutoMapper;
using Web.Api.Core.Interfaces;
namespace Web.Api.Presenters.Grpc;
public class DeleteUserPresenter : PresenterBase<UseCaseResponseMessage>
{
    public Identity.Accounts.DeleteUserResponse Response {get; private set; }
    public DeleteUserPresenter(IMapper mapper) : base(mapper) {}
    public override void Handle(UseCaseResponseMessage response)
    {
        base.Handle(response);
        Response = new Identity.Accounts.DeleteUserResponse() { Id = response.Id, Response = BaseResponse };
    }
}