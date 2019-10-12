using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
using Web.Api.Core.DTO;
using System.Linq;
using AutoMapper;
namespace Web.Api.Presenters.Grpc
{
    public class ChangePasswordPresenter : PresenterBase<UseCaseResponseMessage>
    {
        public Web.Api.Core.Grpc.Response Response {get => BaseResponse; }
        public ChangePasswordPresenter(IMapper mapper) : base(mapper) {}
    }
}