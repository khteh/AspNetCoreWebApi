namespace Web.Api.Models.Request
{
    public record ResetPasswordRequest(string Id, string NewPassword);
}