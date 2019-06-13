using System;
using Web.Api.Core.Domain.Entities;
using Web.Api.Infrastructure.Data;
using Web.Api.Infrastructure.Identity;

namespace Web.Api.IntegrationTests
{
    public static class SeedData
    {
        public static void PopulateTestData(AppIdentityDbContext dbContext)
        {
            dbContext.Users.Add(new AppUser
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
            dbContext.Users.Add(new AppUser
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
            dbContext.SaveChanges();
        }

        public static void PopulateTestData(AppDbContext dbContext)
        {
            var user = new User("Mickey", "Mouse", "41532945-599e-4910-9599-0e7402017fbe", "mickeymouse");
            //user.Id = 1;
            user.AddRefreshToken("cvVsJXuuvb+gTyz+Rk0mBbitkw3AaLgsLecU3cwsUXU=", "127.0.0.1");
            dbContext.Users.Add(user);

            var user1 = new User("Delete", "Me", "7B697F98-AE31-41E7-BE13-20C63314ABF9", "deleteme");
            //user1.Id = 2;
            user1.AddRefreshToken("whatever", "127.0.0.1");

            dbContext.Users.Add(user1);
            dbContext.SaveChanges();
        }
    }
}