using System.Diagnostics.CodeAnalysis;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;

namespace Web.Api.Core.DTO.UseCaseRequests;

public class SignInRequest : IUseCaseRequest<SignInResponse>
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public bool RememberMe { get; set; }
    public bool LockOutOnFailure { get; set; }
    public required string RemoteIpAddress { get; set; }
    public bool IsMobileApp { get; set; }
    [SetsRequiredMembers]
    public SignInRequest(string username, string password, string remoteIP, bool isPersistent = false, bool lockoutOnFailure = true, bool isMobile = false)
    {
        UserName = username;
        Password = password;
        RemoteIpAddress = remoteIP;
        RememberMe = isPersistent;
        LockOutOnFailure = lockoutOnFailure;
        IsMobileApp = isMobile;
    }
}