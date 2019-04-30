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
                Email = "mark@fullstackmark.com",
                NormalizedEmail = "MARK@FULLSTACKMARK.COM",
                PasswordHash = "AQAAAAEAACcQAAAAEKQBX+Qqr/M3qmEcoM3Y/M/8XtVKZ9RnaiXlamue6MIuhOoYONHS7BUHkmOxpF/X3w==",
                SecurityStamp = "YIJZLWUFIIDD3IZSFDD7OQWG6D4QIYPB",
                ConcurrencyStamp = "e432007d-0a54-4332-9212-ca9d7e757275",
                FirstName = "Micky",
                LastName = "Mouse"
            });
           
            dbContext.SaveChanges();
        }

        public static void PopulateTestData(AppDbContext dbContext)
        {
            var user = new User("Mickey", "Mouse", "41532945-599e-4910-9599-0e7402017fbe", "mickeymouse");
            user.AddRefreshToken("rB1afdEe6MWu6TyN8zm58xqt/3KWOLRAah2nHLWcboA=", 1, "127.0.0.1");
            dbContext.Users.Add(user);
            dbContext.SaveChanges();
        }
    }
}
