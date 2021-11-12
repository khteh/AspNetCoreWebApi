using AutoMapper;
using Web.Api.Core.Interfaces;
namespace Web.Api.Presenters.Grpc;
public class LockUserPresenter : PresenterBase<UseCaseResponseMessage>
{
    public Identity.Response Response {get => BaseResponse; }
    public LockUserPresenter(IMapper mapper) : base(mapper) {}
}