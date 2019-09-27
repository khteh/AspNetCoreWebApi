using System.Net;
using System.Linq;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
using Web.Api.Serialization;
using Web.Api.Core.Grpc;
using Google.Protobuf.WellKnownTypes;
using Web.Api.Core.DTO;
namespace Web.Api.Presenters.Grpc
{
    public class FindUserPresenter : IOutputPort<FindUserResponse>
    {
        public Web.Api.Core.Accounts.FindUserResponse Response {get; private set;}
        public void Handle(FindUserResponse response)
        {
            Response = new Web.Api.Core.Accounts.FindUserResponse();
            if (response.Id != null)
                Response.Id = response.Id;
            if (response.User != null && !string.IsNullOrEmpty(response.User.IdentityId))
            {
                Response.User = new Web.Api.Core.Grpc.User() {
                        Id = response.User.Id,
                        FirstName = response.User.FirstName,
                        LastName = response.User.LastName,
                        IdentityId = response.User.IdentityId,
                        UserName = response.User.UserName,
                        Email = response.User.Email,
                        //RefreshTokens = response.User.RefreshTokens
                    };
                if (response.User.RefreshTokens != null && response.User.RefreshTokens.Any())
                    foreach (Web.Api.Core.Domain.Entities.RefreshToken refreshToken in response.User.RefreshTokens)
                    // public RefreshToken(string token, DateTimeOffset expires, int userId,string remoteIpAddress)
                        Response.User.RefreshTokens.Add(new Web.Api.Core.Grpc.RefreshToken() {
                            Token = refreshToken.Token, 
                            Expires = Timestamp.FromDateTimeOffset(refreshToken.Expires), 
                            UserId = refreshToken.UserId, 
                            RemoteIpAddress = refreshToken.RemoteIpAddress});
            }
            Response.Response = new Web.Api.Core.Grpc.Response();
            Response.Response.Success = response.Success;
            if (response.Errors != null && response.Errors.Any())
                foreach (Web.Api.Core.DTO.Error error in response.Errors)
                    Response.Response.Errors.Add(new Web.Api.Core.Grpc.Error() {Code = error.Code, Description = error.Description});
        }
    }
}