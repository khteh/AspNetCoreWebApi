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
            Response = new Web.Api.Core.Accounts.FindUserResponse() { Response = base.Response };
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
        }
    }
}