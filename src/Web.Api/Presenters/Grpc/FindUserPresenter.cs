using AutoMapper;
using Web.Api.Core.DTO.UseCaseResponses;
namespace Web.Api.Presenters.Grpc;
public class FindUserPresenter : PresenterBase<FindUserResponse>
{
    public Identity.Accounts.FindUserResponse Response { get; private set; }
    public FindUserPresenter(IMapper mapper) : base(mapper) { }
    public override async Task Handle(FindUserResponse response)
    {
        await base.Handle(response);
        Response = new Identity.Accounts.FindUserResponse() { Response = BaseResponse };
        if (response.Id != null)
            Response.Id = response.Id;
        if (response.User != null && !string.IsNullOrEmpty(response.User.IdentityId))
            Response.User = _mapper.Map<Identity.User>(response.User);
    }
}