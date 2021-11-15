using Microsoft.EntityFrameworkCore;
using Web.Api.Infrastructure.Shared;
/* This is used by EF migration. Do NOT remove!
 * https://docs.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli
 */
namespace Web.Api.Infrastructure.Identity;
public class AppIdentityDbContextFactory : DesignTimeDbContextFactoryBase<AppIdentityDbContext>
{
    protected override AppIdentityDbContext CreateNewInstance(DbContextOptions<AppIdentityDbContext> options) => new AppIdentityDbContext(options);
}