using AutoMapper;
using Web.Api.Core.Interfaces;
namespace Web.Api.Presenters.Grpc;
public class UserPresenter : PresenterBase<UseCaseResponseMessage>
{
    public Identity.Accounts.UserResponse Response { get; private set; }
    public UserPresenter(IMapper mapper) : base(mapper) { }
    public override async Task Handle(UseCaseResponseMessage response)
    {
        await base.Handle(response);
        Response = new Identity.Accounts.UserResponse() { Id = response.Id, Response = BaseResponse };
    }
}