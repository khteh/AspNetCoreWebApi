namespace Web.Api.Models.Request
{
    public record RegisterUserRequest
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public string UserName { get; init; }
        public string Password { get; init; }
        public RegisterUserRequest(string firstName, string lastName, string email, string userName, string password) => (FirstName, LastName, Email, UserName, Password) = (firstName, lastName, email, userName, password);
    }
}