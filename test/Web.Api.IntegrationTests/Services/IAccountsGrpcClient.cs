using System;
using System.Threading.Tasks;
using Web.Api.Core.Accounts;
using Web.Api.Core.Grpc;
namespace Web.Api.IntegrationTests.Services
{
    interface IAccountsGrpcClient<TMessage, TResponse> where TResponse : class
    {
        Task<RegisterUserResponse> Register (RegisterUserRequest request);
        Task<Response> ChangePassword (ChangePasswordRequest request);
        Task<Response> ResetPassword (ResetPasswordRequest request);
        Task<Response> Lock (StringInputParameter request);
        Task<Response> UnLock (StringInputParameter request);
        Task<DeleteUserResponse> Delete (StringInputParameter request);
        Task<FindUserResponse> FindById (StringInputParameter request);
        Task<FindUserResponse> FindByUserName (StringInputParameter request);
        Task<FindUserResponse> FindByEmail (StringInputParameter request);
    }
}