using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;

namespace Web.Api.Core.DTO.UseCaseRequests
{
    public class DeleteUserRequest : IUseCaseRequest<DeleteUserResponse>
    {
        public string UserName { get; }
        public DeleteUserRequest(string username) => UserName = username;
    }
}