using System.Threading.Tasks;
using Web.Api.Core.Domain.Entities;
using Web.Api.Core.Dto.GatewayResponses.Repositories;

namespace Web.Api.Core.Interfaces.Gateways.Repositories
{
    public interface IUserRepository  : IRepository<User>
    {
        Task<CreateUserResponse> Create(string firstName, string lastName, string email, string userName, string password);
        Task<DeleteUserResponse> Delete(string username);
        Task<FindUserResponse> FindById(string id);
        Task<User> FindUserByName(string userName);
        Task<FindUserResponse> FindByName(string userName);
        Task<FindUserResponse> FindByEmail(string email);
        Task<bool> CheckPassword(string username, string password);
        Task<PasswordResponse> ChangePassword(string id, string oldPassword, string newPassword);
    }
}