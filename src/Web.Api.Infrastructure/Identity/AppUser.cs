//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Api.Infrastructure.Identity
{
    public class AppUser : IdentityUser
    {
        // Add additional profile data for application users by adding properties to this class
        [MaxLength(85)]
        public override string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public AppUser() { }
        public AppUser(string username, string email, string firstName, string lastName, string passwordHash, DateTimeOffset created, DateTimeOffset modified)
        {
            UserName = username;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Created = created;
            Modified = modified;
            PasswordHash = passwordHash;
        }
    }
}