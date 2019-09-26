using System.Net;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Serialization;

namespace Web.Api.Presenters.Grpc
{
    public class FindUserPresenter : PresenterBase<Web.Api.Core.DTO.UseCaseResponses.FindUserResponse, Web.Api.Core.Accounts.FindUserResponse>
    {
        public override void Handle(Web.Api.Core.DTO.UseCaseResponses.FindUserResponse response) =>
            Response = new Web.Api.Core.Accounts.FindUserResponse() {
                Id = response.Id,
                User = response.User.ToByteArray(),
                Response = new Web.Api.Core.Grpc.Response() {
                    Success = response.Success,
                    Errors = response.Errors
                }
            };
    }
}