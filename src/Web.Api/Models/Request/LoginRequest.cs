namespace Web.Api.Models.Request;
public record LogInRequest(string UserName, string Password, bool RememberMe = false);