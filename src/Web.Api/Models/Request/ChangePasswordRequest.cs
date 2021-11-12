namespace Web.Api.Models.Request;
public record ChangePasswordRequest(string Id, string Password, string NewPassword);