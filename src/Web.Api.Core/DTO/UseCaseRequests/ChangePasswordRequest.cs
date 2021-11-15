using Web.Api.Core.Interfaces;
namespace Web.Api.Core.DTO.UseCaseRequests;
public class ChangePasswordRequest : IUseCaseRequest<UseCaseResponseMessage>
{
    public string IdentityId {get;set;}
    public string OldPassword {get;set;}
    public string NewPassword {get;set;}
    public ChangePasswordRequest(string id, string oldPassword, string newPassword)
    {
        IdentityId = id;
        OldPassword = oldPassword;
        NewPassword = newPassword;
    }
}