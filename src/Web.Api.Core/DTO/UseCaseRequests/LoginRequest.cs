using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;

namespace Web.Api.Core.DTO.UseCaseRequests
{
    public class LoginRequest : IUseCaseRequest<LoginResponse>
    {
        public string UserName { get; }
        public string Password { get; }
        public string RemoteIpAddress { get; }
        public bool RememberMe { get; set; }
        public bool LockoutOnFailure {get; set; }
        public bool CheckLoginAllowed {get; set; }
        public LoginRequest(string userName, string password, string remoteIpAddress)
        {
            UserName = userName;
            Password = password;
            RemoteIpAddress = remoteIpAddress;
        }
        public LoginRequest(string username, string password, string remoteIpAddress, bool rememberMe, bool lockoutOnFailure, bool checkLoginAllowed)
        {
            UserName = username;
            Password = password;
            RemoteIpAddress = remoteIpAddress;
            RememberMe = rememberMe;
            LockoutOnFailure = lockoutOnFailure;
            CheckLoginAllowed = checkLoginAllowed;
        }
    }
}