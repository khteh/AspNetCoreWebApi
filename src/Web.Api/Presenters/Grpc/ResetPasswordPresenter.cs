using AutoMapper;
using Web.Api.Core.Interfaces;
namespace Web.Api.Presenters.Grpc;
public class ResetPasswordPresenter : PresenterBase<UseCaseResponseMessage>
{
    public Identity.Response Response {get => BaseResponse; }
    public ResetPasswordPresenter(IMapper mapper) : base(mapper) {}
}