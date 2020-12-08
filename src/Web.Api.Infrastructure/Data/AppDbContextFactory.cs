using Microsoft.EntityFrameworkCore;
using Web.Api.Infrastructure.Shared;
/* This is used by EF migration. Do NOT remove!
 * https://docs.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli
 */
namespace Web.Api.Infrastructure.Data
{
    public class AppDbContextFactory : DesignTimeDbContextFactoryBase<AppDbContext>
    {
        protected override AppDbContext CreateNewInstance(DbContextOptions<AppDbContext> options) => new AppDbContext(options);
    }
}