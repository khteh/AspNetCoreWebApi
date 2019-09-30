using System.Net;
using System.Linq;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Serialization;
using Web.Api.Core.Grpc;
using Google.Protobuf.WellKnownTypes;
using Web.Api.Core.DTO;
using AutoMapper;
namespace Web.Api.Presenters.Grpc
{
    public class FindUserPresenter  : PresenterBase<FindUserResponse>
    {
        public Web.Api.Core.Accounts.FindUserResponse Response {get; private set;}
        public FindUserPresenter(IMapper mapper) : base(mapper) {}
        public override void Handle(FindUserResponse response)
        {
            base.Handle(response);
            Response = new Web.Api.Core.Accounts.FindUserResponse() { Response = base.BaseResponse };
            if (response.Id != null)
                Response.Id = response.Id;
            if (response.User != null && !string.IsNullOrEmpty(response.User.IdentityId))
                Response.User = _mapper.Map<Web.Api.Core.Grpc.User>(response.User);
        }
    }
}