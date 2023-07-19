using System.Linq;
using Web.Api.Core.Domain.Entities;
using Web.Api.Infrastructure.Data;
using Web.Api.Infrastructure.Identity;

namespace Web.Api.IntegrationTests;
public static class SeedData
{
    public static void CleanUpTestData(AppIdentityDbContext dbIdentityContext, AppDbContext dbContext)
    {
        AppUser appUser = dbIdentityContext.Users.FirstOrDefault(i => i.UserName.Equals("mickeymouse"));
        if (appUser != null)
        {
            User user = dbContext.Users.FirstOrDefault(i => i.IdentityId.Equals(appUser.Id));
            dbIdentityContext.Users.Remove(appUser);
            if (user != null)
                dbContext.Users.Remove(user);
        }
        appUser = dbIdentityContext.Users.FirstOrDefault(i => i.UserName.Equals("user1"));
        if (appUser != null)
        {
            User user = dbContext.Users.FirstOrDefault(i => i.IdentityId.Equals(appUser.Id));
            dbIdentityContext.Users.Remove(appUser);
            if (user != null)
                dbContext.Users.Remove(user);
        }
        appUser = dbIdentityContext.Users.FirstOrDefault(i => i.UserName.Equals("user2"));
        if (appUser != null)
        {
            User user = dbContext.Users.FirstOrDefault(i => i.IdentityId.Equals(appUser.Id));
            dbIdentityContext.Users.Remove(appUser);
            if (user != null)
                dbContext.Users.Remove(user);
        }
        appUser = dbIdentityContext.Users.FirstOrDefault(i => i.UserName.Equals("deleteme"));
        if (appUser != null)
        {
            User user = dbContext.Users.FirstOrDefault(i => i.IdentityId.Equals(appUser.Id));
            dbIdentityContext.Users.Remove(appUser);
            if (user != null)
                dbContext.Users.Remove(user);
        }
        appUser = dbIdentityContext.Users.FirstOrDefault(i => i.UserName.Equals("johndoe"));
        if (appUser != null)
        {
            User user = dbContext.Users.FirstOrDefault(i => i.IdentityId.Equals(appUser.Id));
            dbIdentityContext.Users.Remove(appUser);
            if (user != null)
                dbContext.Users.Remove(user);
        }
        dbIdentityContext.SaveChanges();
        dbContext.SaveChanges();
    }
    public static void PopulateTestData(AppIdentityDbContext dbIdentityContext, AppDbContext dbContext)
    {
        AppUser appUser = dbIdentityContext.Users.FirstOrDefault(i => i.UserName.Equals("mickeymouse"));
        if (appUser == null)
            dbIdentityContext.Users.Add(new AppUser
            {
                Id = "41532945-599e-4910-9599-0e7402017fbe",
                UserName = "mickeymouse",
                NormalizedUserName = "MICKEYMOUSE",
                Email = "mickey@mouse.com",
                NormalizedEmail = "MICKEY@MOUSE.COM",
                PasswordHash = "AQAAAAEAACcQAAAAEKof9i25YVCmttE29Fr40Pa4LLOeQeQQQP2NRLv0IGRapPY1pkCbsleOqEHKEtuEtA==",
                SecurityStamp = "YIJZLWUFIIDD3IZSFDD7OQWG6D4QIYPB",
                ConcurrencyStamp = "e432007d-0a54-4332-9212-ca9d7e757275",
                FirstName = "Micky",
                LastName = "Mouse"
            });
        appUser = dbIdentityContext.Users.FirstOrDefault(i => i.UserName.Equals("deleteme"));
        if (appUser == null)
            dbIdentityContext.Users.Add(new AppUser
            {
                Id = "7B697F98-AE31-41E7-BE13-20C63314ABF9",
                UserName = "deleteme",
                NormalizedUserName = "DELETEME",
                Email = "delete@me.com",
                NormalizedEmail = "DELETE@ME.COM",
                PasswordHash = "AQAAAAEAACcQAAAAEKof9i25YVCmttE29Fr40Pa4LLOeQeQQQP2NRLv0IGRapPY1pkCbsleOqEHKEtuEtA==",
                SecurityStamp = "YIJZLWUFIIDD3IZSFDD7OQWG6D4QIYPB",
                ConcurrencyStamp = "8313C9A6-DB7E-469D-8195-732D5808F731",
                FirstName = "Delete",
                LastName = "Me"
            });

        User user = dbContext.Users.FirstOrDefault(i => i.IdentityId.Equals("41532945-599e-4910-9599-0e7402017fbe"));
        if (user == null)
        {
            user = new User("Mickey", "Mouse", "41532945-599e-4910-9599-0e7402017fbe", "mickeymouse");
            //user.Id = 1;
            user.AddRefreshToken("cvVsJXuuvb+gTyz+Rk0mBbitkw3AaLgsLecU3cwsUXU=", "127.0.0.1");
            dbContext.Users.Add(user);
        }

        user = dbContext.Users.FirstOrDefault(i => i.IdentityId.Equals("7B697F98-AE31-41E7-BE13-20C63314ABF9"));
        if (user == null)
        {
            user = new User("Delete", "Me", "7B697F98-AE31-41E7-BE13-20C63314ABF9", "deleteme");
            //user1.Id = 2;
            user.AddRefreshToken("whatever", "127.0.0.1");
            dbContext.Users.Add(user);
        }
        dbIdentityContext.SaveChanges();
        dbContext.SaveChanges();
    }
    public static void CleanUpGrpcTestData(AppIdentityDbContext dbIdentityContext, AppDbContext dbContext)
    {
        AppUser appUser = dbIdentityContext.Users.FirstOrDefault(i => i.UserName.Equals("mickeymousegrpc"));
        if (appUser != null)
        {
            User user = dbContext.Users.FirstOrDefault(i => i.IdentityId.Equals(appUser.Id));
            dbIdentityContext.Users.Remove(appUser);
            if (user != null)
                dbContext.Users.Remove(user);
        }

        appUser = dbIdentityContext.Users.FirstOrDefault(i => i.UserName.Equals("user1grpc"));
        if (appUser != null)
        {
            User user = dbContext.Users.FirstOrDefault(i => i.IdentityId.Equals(appUser.Id));
            dbIdentityContext.Users.Remove(appUser);
            if (user != null)
                dbContext.Users.Remove(user);
        }

        appUser = dbIdentityContext.Users.FirstOrDefault(i => i.UserName.Equals("user2grpc"));
        if (appUser != null)
        {
            User user = dbContext.Users.FirstOrDefault(i => i.IdentityId.Equals(appUser.Id));
            dbIdentityContext.Users.Remove(appUser);
            if (user != null)
                dbContext.Users.Remove(user);
        }

        appUser = dbIdentityContext.Users.FirstOrDefault(i => i.UserName.Equals("johndoegrpc"));
        if (appUser != null)
        {
            User user = dbContext.Users.FirstOrDefault(i => i.IdentityId.Equals(appUser.Id));
            dbIdentityContext.Users.Remove(appUser);
            if (user != null)
                dbContext.Users.Remove(user);
        }
        dbIdentityContext.SaveChanges();
        dbContext.SaveChanges();
    }
    public static void PopulateGrpcTestData(AppIdentityDbContext dbIdentityContext, AppDbContext dbContext)
    {
        dbIdentityContext.Users.Add(new AppUser
        {
            Id = "CE73A87D-0AA6-4191-B65B-6B49F333E316",
            UserName = "mickeymousegrpc",
            NormalizedUserName = "MICKEYMOUSEGRPC",
            Email = "mickeygrpc@mouse.com",
            NormalizedEmail = "MICKEYGRPC@MOUSE.COM",
            PasswordHash = "AQAAAAEAACcQAAAAEKof9i25YVCmttE29Fr40Pa4LLOeQeQQQP2NRLv0IGRapPY1pkCbsleOqEHKEtuEtA==",
            SecurityStamp = "YIJZLWUFIIDD3IZSFDD7OQWG6D4QIYPB",
            ConcurrencyStamp = "e432007d-0a54-4332-9212-ca9d7e757275",
            FirstName = "Micky Grpc",
            LastName = "Mouse"
        });
        dbIdentityContext.Users.Add(new AppUser
        {
            Id = "3ABE9D63-777C-4865-8FA0-A53A657313D5",
            UserName = "deletemegrpc",
            NormalizedUserName = "DELETEMEGRPC",
            Email = "deletegrpc@me.com",
            NormalizedEmail = "DELETEGRPC@ME.COM",
            PasswordHash = "AQAAAAEAACcQAAAAEKof9i25YVCmttE29Fr40Pa4LLOeQeQQQP2NRLv0IGRapPY1pkCbsleOqEHKEtuEtA==",
            SecurityStamp = "YIJZLWUFIIDD3IZSFDD7OQWG6D4QIYPB",
            ConcurrencyStamp = "8313C9A6-DB7E-469D-8195-732D5808F731",
            FirstName = "Delete Grpc",
            LastName = "Me"
        });

        var user = new User("Mickey Grpc", "Mouse", "CE73A87D-0AA6-4191-B65B-6B49F333E316", "mickeymousegrpc");
        //user.Id = 1;
        user.AddRefreshToken("cvVsJXuuvb+gTyz+Rk0mBbitkw3AaLgsLecU3cwsUXU=", "127.0.0.1");
        dbContext.Users.Add(user);

        var user1 = new User("Delete Grpc", "Me", "3ABE9D63-777C-4865-8FA0-A53A657313D5", "deletemegrpc");
        //user1.Id = 2;
        user1.AddRefreshToken("whatever", "127.0.0.1");
        dbContext.Users.Add(user1);
        dbIdentityContext.SaveChanges();
        dbContext.SaveChanges();
    }
}