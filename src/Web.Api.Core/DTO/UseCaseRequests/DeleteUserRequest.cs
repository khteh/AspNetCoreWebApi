using Web.Api.Core.Interfaces;
namespace Web.Api.Core.DTO.UseCaseRequests;
public class DeleteUserRequest : IUseCaseRequest<UseCaseResponseMessage>
{
    public string UserName { get; }
    public DeleteUserRequest(string username) => UserName = username;
}