using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Api.Core.Shared;
namespace Web.Api.Core.Interfaces.Gateways.Repositories;
public interface IRepository<T> where T : BaseEntity
{
    Task<T> GetById(int id);
    Task<List<T>> ListAll();
    Task<T> GetSingleBySpec(ISpecification<T> spec);
    Task<long> Count(ISpecification<T> spec, int page = -1, int pageSize = 100);
    Task<List<T>> List(ISpecification<T> spec, int page = -1, int pageSize = 100);
    Task<T> Add(T entity);
    Task Update(T entity);
    Task Delete(T entity);
}