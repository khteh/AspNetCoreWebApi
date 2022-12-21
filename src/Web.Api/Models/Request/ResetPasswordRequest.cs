namespace Web.Api.Models.Request;
public record ResetPasswordRequest(string Id, string Email, string NewPassword, string Code, bool IsFirstLogin = false);