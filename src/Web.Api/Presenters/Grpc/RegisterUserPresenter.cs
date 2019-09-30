using System.Net;
using System.Linq;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Serialization;
using Web.Api.Core.DTO;
using AutoMapper;
namespace Web.Api.Presenters.Grpc
{
    public sealed class RegisterUserPresenter : PresenterBase<UseCaseResponseMessage>
    {
        public Web.Api.Core.Accounts.RegisterUserResponse Response {get; private set;}
        public RegisterUserPresenter(IMapper mapper) : base(mapper) {}
        public override void Handle(UseCaseResponseMessage response)
        {
            base.Handle(response);
            Response = new Web.Api.Core.Accounts.RegisterUserResponse() { Id = response.Id, Response = base.Response };
        }
    }
}