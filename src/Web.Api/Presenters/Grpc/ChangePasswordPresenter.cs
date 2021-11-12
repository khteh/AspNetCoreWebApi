using AutoMapper;
using Web.Api.Core.Interfaces;
namespace Web.Api.Presenters.Grpc;
public class ChangePasswordPresenter : PresenterBase<UseCaseResponseMessage>
{
    public Identity.Response Response {get => BaseResponse; }
    public ChangePasswordPresenter(IMapper mapper) : base(mapper) {}
}