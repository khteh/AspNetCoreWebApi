

namespace Web.Api.Models.Request
{
    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public LoginRequest(string userName, string password, bool rememberMe = false)
        {
            UserName = userName;
            Password = password;
            RememberMe = rememberMe;
        }
    }
}