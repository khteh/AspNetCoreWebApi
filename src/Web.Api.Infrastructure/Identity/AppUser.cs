//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace Web.Api.Infrastructure.Identity;

public class AppUser : IdentityUser
{
    // Add additional profile data for application users by adding properties to this class
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public AppUser() { }
    public AppUser(string username, string email, string firstName, string lastName) => (UserName, Email, FirstName, LastName) = (username, email, firstName, lastName);
}