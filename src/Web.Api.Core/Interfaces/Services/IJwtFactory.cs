using System.Threading.Tasks;
using Web.Api.Core.DTO;

namespace Web.Api.Core.Interfaces.Services
{
    public interface IJwtFactory
    {
        Task<AccessToken> GenerateEncodedToken(string id, string userName);
    }
}
