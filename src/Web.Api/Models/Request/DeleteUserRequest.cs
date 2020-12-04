namespace Web.Api.Models.Request
{
    public record DeleteUserRequest
    {
        public string UserName { get; init; }
    }
}