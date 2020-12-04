namespace Web.Api.Models.Request
{
    public record LoginRequest
    {
        public string UserName { get; init; }
        public string Password { get; init; }
        public bool RememberMe { get; init; }
        public LoginRequest(string userName, string password, bool rememberMe = false) => (UserName, Password, RememberMe) = (userName, password, rememberMe);
    }
}