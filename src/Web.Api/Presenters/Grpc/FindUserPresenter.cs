using System.Net;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Serialization;
using Web.Api.Core.Grpc;
namespace Web.Api.Presenters.Grpc
{
    public class FindUserPresenter : PresenterBase<Web.Api.Core.DTO.UseCaseResponses.FindUserResponse, Web.Api.Core.Accounts.FindUserResponse>
    {
        public override void Handle(Web.Api.Core.DTO.UseCaseResponses.FindUserResponse response) =>
            Response = new Web.Api.Core.Accounts.FindUserResponse() {
                Id = response.Id,
                User = new Web.Api.Core.Grpc.User() {
                    Id = response.User.Id,
                    FirstName = response.User.FirstName,
                    LastName = response.User.LastName,
                    IdentityId = response.User.IdentityId,
                    UserName = response.User.UserName,
                    Email = response.User.Email,
                    RefreshTokens = response.User.RefreshTokens
                },
                Response = new Web.Api.Core.Grpc.Response() {
                    Success = response.Success,
                    Errors = response.Errors
                }
            };
    }
}