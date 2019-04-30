dotnet ef migrations add initial --context AppIdentityDbContext
dotnet ef migrations add initial --context AppDbContext
dotnet ef database update --context AppIdentityDbContext
dotnet ef database update --context AppDbContext