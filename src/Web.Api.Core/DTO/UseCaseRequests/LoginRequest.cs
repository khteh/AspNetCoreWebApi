using Web.Api.Core.DTO.UseCaseResponses;
using Web.Api.Core.Interfaces;
namespace Web.Api.Core.DTO.UseCaseRequests;
public class LogInRequest : IUseCaseRequest<LogInResponse>
{
    public string UserName { get; }
    public string Password { get; }
    public string RemoteIpAddress { get; }
    public LogInRequest(string userName, string password, string remoteIpAddress)
    {
        UserName = userName;
        Password = password;
        RemoteIpAddress = remoteIpAddress;
    }
}