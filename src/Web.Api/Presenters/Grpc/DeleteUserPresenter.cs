using System.Net;
using System.Linq;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Models.Response;
using Web.Api.Serialization;
using Web.Api.Core.DTO;
using AutoMapper;
namespace Web.Api.Presenters.Grpc
{
    public class DeleteUserPresenter : PresenterBase<UseCaseResponseMessage>
    {
        public Web.Api.Core.Accounts.DeleteUserResponse Response {get; private set; }
        public DeleteUserPresenter(IMapper mapper) : base(mapper) {}
        public override void Handle(UseCaseResponseMessage response)
        {
            base.Handle(response);
            Response = new Web.Api.Core.Accounts.DeleteUserResponse() { Id = response.Id, Response = base.BaseResponse };
        }
    }
}