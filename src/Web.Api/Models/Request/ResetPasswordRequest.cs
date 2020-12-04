namespace Web.Api.Models.Request
{
    public record ResetPasswordRequest
    {
        public string Id { get; init; }
        public string NewPassword { get; init; }
        public ResetPasswordRequest(string id, string newPassword) => (Id, NewPassword) = (id, newPassword);
    }
}