using System.Net;
using System.Linq;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Serialization;
using Web.Api.Identity;
using Google.Protobuf.WellKnownTypes;
using Web.Api.Core.DTO;
using AutoMapper;
namespace Web.Api.Presenters.Grpc
{
    public class FindUserPresenter  : PresenterBase<FindUserResponse>
    {
        public Web.Api.Identity.Accounts.FindUserResponse Response {get; private set;}
        public FindUserPresenter(IMapper mapper) : base(mapper) {}
        public override void Handle(FindUserResponse response)
        {
            base.Handle(response);
            Response = new Web.Api.Identity.Accounts.FindUserResponse() { Response = BaseResponse };
            if (response.Id != null)
                Response.Id = response.Id;
            if (response.User != null && !string.IsNullOrEmpty(response.User.IdentityId))
                Response.User = _mapper.Map<Web.Api.Identity.User>(response.User);
        }
    }
}