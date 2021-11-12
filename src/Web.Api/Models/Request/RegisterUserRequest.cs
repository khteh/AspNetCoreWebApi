namespace Web.Api.Models.Request;
public record RegisterUserRequest(string FirstName, string LastName, string Email, string UserName, string Password);