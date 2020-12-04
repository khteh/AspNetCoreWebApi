namespace Web.Api.Models.Request
{
    public record ChangePasswordRequest
    {
        public string Id { get; init; }
        public string Password { get; init; }
        public string NewPassword { get; init; }
        public ChangePasswordRequest(string id, string password, string newPassword) => (Id, Password, NewPassword) = (id, password, newPassword);
    }
}