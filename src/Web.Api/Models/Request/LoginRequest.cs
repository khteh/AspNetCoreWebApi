namespace Web.Api.Models.Request
{
    public record LoginRequest(string UserName, string Password, bool RememberMe = false);
}