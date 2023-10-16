using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Api.Core.Interfaces.Gateways.Repositories;
using Web.Api.Core.Shared;
namespace Web.Api.Infrastructure.Data.Repositories;
public abstract class EfRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _appDbContext;
    protected EfRepository(AppDbContext appDbContext) => _appDbContext = appDbContext;
    public virtual async Task<T> GetById(int id) => await _appDbContext.Set<T>().FindAsync(id);
    public virtual async Task<List<T>> ListAll() => await _appDbContext.Set<T>().ToListAsync();
    public virtual async Task<T> GetSingleBySpec(ISpecification<T> spec)
    {
        IQueryable<T> result = await Aggregate(spec);
        return result.FirstOrDefault();
    }
    public virtual async Task<long> Count(ISpecification<T> spec, int page = -1, int pageSize = 100) => (await Aggregate(spec, page, pageSize)).Count();
    public virtual async Task<List<T>> List(ISpecification<T> spec, int page = -1, int pageSize = 100)
    {
        IQueryable<T> result = await Aggregate(spec, page, pageSize);
        return await result.ToListAsync();
    }
    public virtual async Task<T> Add(T entity)
    {
        _appDbContext.Set<T>().Add(entity);
        await _appDbContext.SaveChangesAsync();
        return entity;
    }
    public virtual async Task Update(T entity)
    {
        _appDbContext.Entry(entity).State = EntityState.Modified;
        await _appDbContext.SaveChangesAsync();
    }
    public virtual async Task Delete(T entity)
    {
        _appDbContext.Set<T>().Remove(entity);
        await _appDbContext.SaveChangesAsync();
    }
    private async Task<IQueryable<T>> Aggregate(ISpecification<T> spec, int page = -1, int pageSize = 100)
    {
        // fetch a Queryable that includes all expression-based includes
        IQueryable<T> queryableResultWithIncludes = spec.Includes
                .Aggregate(_appDbContext.Set<T>().AsQueryable(),
                    (current, include) => current.Include(include));
        // modify the IQueryable to include any string-based include statements
        IQueryable<T> secondaryResult = spec.IncludeStrings
                .Aggregate(queryableResultWithIncludes,
                    (current, include) => current.Include(include));
        IQueryable<T> orderedResult = secondaryResult.Where(spec.Criteria).AsSplitQuery();
        if (spec.OrderBy != null)
            orderedResult = orderedResult.OrderBy(spec.OrderBy).AsSplitQuery();
        else if (spec.OrderByDescending != null)
            orderedResult = orderedResult.OrderByDescending(spec.OrderByDescending).AsSplitQuery();
        return page >= 0 && pageSize > 0 ? orderedResult.Skip(page * pageSize).Take(pageSize) : orderedResult;
    }
}