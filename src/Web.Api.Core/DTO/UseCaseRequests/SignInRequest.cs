using System;
using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;

namespace Web.Api.Core.DTO.UseCaseRequests
{
    public class SignInRequest : IUseCaseRequest<SignInResponse>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public bool LockOutOnFailure {get; set; }
        public bool IsMobileApp {get; set; }
        public SignInRequest() {}
        public SignInRequest(string username, string password, bool isPersistent = false, bool lockoutOnFailure = true,  bool isMobile = false)
        {
            UserName = username;
            Password = password;
            RememberMe = isPersistent;
            LockOutOnFailure = lockoutOnFailure;
            IsMobileApp = isMobile;
        }
    }
}