using AutoMapper;
using Web.Api.Core.Interfaces;
namespace Web.Api.Presenters.Grpc;
public class UserPresenter : PresenterBase<UseCaseResponseMessage>
{
    public Identity.Accounts.UserResponse Response { get; private set; }
    public UserPresenter(IMapper mapper) : base(mapper) { }
    public override void Handle(UseCaseResponseMessage response)
    {
        base.Handle(response);
        Response = new Identity.Accounts.UserResponse() { Id = response.Id, Response = BaseResponse };
    }
}