using AutoMapper;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;

namespace Web.Api.Presenters.Grpc;
public class EmailConfirmationPresenter : PresenterBase<UseCaseResponseMessage>
{
    public Identity.Response Response {get => BaseResponse; }
    public EmailConfirmationPresenter(IMapper mapper) : base(mapper) {}
}