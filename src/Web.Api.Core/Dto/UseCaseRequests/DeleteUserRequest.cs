using Web.Api.Core.Dto.UseCaseResponses;
using Web.Api.Core.Interfaces;

namespace Web.Api.Core.Dto.UseCaseRequests
{
    public class DeleteUserRequest : IUseCaseRequest<DeleteUserResponse>
    {
        public string UserName { get; }
        public DeleteUserRequest(string username) => UserName = username;
    }
}