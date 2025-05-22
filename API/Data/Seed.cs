using System;
using System.Text.Json;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{
    public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        if (await userManager.Users.AnyAsync()) return;

        var roles = new List<AppRole>
        {
            new() {Name = "Super Admin"},
            new() {Name = "Admin"},
            new() {Name = "User"},
        };

        foreach (var role in roles)
        {
            await roleManager.CreateAsync(role);
        }

        var admin = new AppUser
        {
            UserName = "gab@example.com",
            Email = "gab@example.com",
        };

        await userManager.CreateAsync(admin, "Pa$$w0rd");
        await userManager.AddToRolesAsync(admin, ["Super Admin"]);
    }

    public static async Task SeedUserIdentities(DataContext context)
    {
        if (await context.UserIdentities.AnyAsync()) return;

        var userIdentities = new UserIdentity[]
        {
                new UserIdentity
                {
                    UserId = "1",
                    FullName = "Elamae Cleofe",
                    Email = "ela@example.com",
                    SourceSystem = "ActiveDirectory",
                    LastUpdated = DateTime.Now,
                    IsActive = true
                },
                new UserIdentity
                {
                    UserId = "2",
                    FullName = "Jane Smith",
                    Email = "jane.smith@example.com",
                    SourceSystem = "LDAP",
                    LastUpdated = DateTime.Now,
                    IsActive = true
                },
                new UserIdentity
                {
                    UserId = "3",
                    FullName = "John Doe",
                    Email = "john.doe@example.com",
                    SourceSystem = "Google",
                    LastUpdated = DateTime.Now,
                    IsActive = false
                },
                new UserIdentity
                {
                    UserId = "4",
                    FullName = "Alice Johnson",
                    Email = "alice.johnson@example.com",
                    SourceSystem = "Okta",
                    LastUpdated = DateTime.Now,
                    IsActive = true
                },
                new UserIdentity
                {
                    UserId = "5",
                    FullName = "Bob Brown",
                    Email = "bob.brown@example.com",
                    SourceSystem = "AzureAD",
                    LastUpdated = DateTime.Now,
                    IsActive = true
                },
                new UserIdentity
                {
                    UserId = "6",
                    FullName = "Carol Davis",
                    Email = "carol.davis@example.com",
                    SourceSystem = "ActiveDirectory",
                    LastUpdated = DateTime.Now,
                    IsActive = true
                },
                new UserIdentity
                {
                    UserId = "7",
                    FullName = "David Lee",
                    Email = "david.lee@example.com",
                    SourceSystem = "LDAP",
                    LastUpdated = DateTime.Now,
                    IsActive = true
                },
                new UserIdentity
                {
                    UserId = "8",
                    FullName = "Eve White",
                    Email = "eve.white@example.com",
                    SourceSystem = "Google",
                    LastUpdated = DateTime.Now,
                    IsActive = false
                },
                new UserIdentity
                {
                    UserId = "9",
                    FullName = "Frank Taylor",
                    Email = "frank.taylor@example.com",
                    SourceSystem = "Okta",
                    LastUpdated = DateTime.Now,
                    IsActive = true
                },
                new UserIdentity
                {
                    UserId = "10",
                    FullName = "George Martin",
                    Email = "george.martin@example.com",
                    SourceSystem = "AzureAD",
                    LastUpdated = DateTime.Now,
                    IsActive = true
                },
                new UserIdentity
                {
                    UserId = "11",
                    FullName = "Hannah Williams",
                    Email = "hannah.williams@example.com",
                    SourceSystem = "ActiveDirectory",
                    LastUpdated = DateTime.Now,
                    IsActive = true
                },
                new UserIdentity
                {
                    UserId = "12",
                    FullName = "Ian McDonald",
                    Email = "ian.mcdonald@example.com",
                    SourceSystem = "LDAP",
                    LastUpdated = DateTime.Now,
                    IsActive = true
                },
                new UserIdentity
                {
                    UserId = "13",
                    FullName = "Julia Brown",
                    Email = "julia.brown@example.com",
                    SourceSystem = "Google",
                    LastUpdated = DateTime.Now,
                    IsActive = false
                },
                new UserIdentity
                {
                    UserId = "14",
                    FullName = "Kevin White",
                    Email = "kevin.white@example.com",
                    SourceSystem = "Okta",
                    LastUpdated = DateTime.Now,
                    IsActive = true
                },
                new UserIdentity
                {
                    UserId = "15",
                    FullName = "Lisa Jackson",
                    Email = "lisa.jackson@example.com",
                    SourceSystem = "AzureAD",
                    LastUpdated = DateTime.Now,
                    IsActive = true
                },
                new UserIdentity
                {
                    UserId = "16",
                    FullName = "Mark Lee",
                    Email = "mark.lee@example.com",
                    SourceSystem = "ActiveDirectory",
                    LastUpdated = DateTime.Now,
                    IsActive = true
                },
                new UserIdentity
                {
                    UserId = "17",
                    FullName = "Natalie Davis",
                    Email = "natalie.davis@example.com",
                    SourceSystem = "LDAP",
                    LastUpdated = DateTime.Now,
                    IsActive = true
                },
                new UserIdentity
                {
                    UserId = "18",
                    FullName = "Olivia Martin",
                    Email = "olivia.martin@example.com",
                    SourceSystem = "Google",
                    LastUpdated = DateTime.Now,
                    IsActive = false
                },
                new UserIdentity
                {
                    UserId = "19",
                    FullName = "Patrick Taylor",
                    Email = "patrick.taylor@example.com",
                    SourceSystem = "Okta",
                    LastUpdated = DateTime.Now,
                    IsActive = true
                },
                new UserIdentity
                {
                    UserId = "20",
                    FullName = "Quincy White",
                    Email = "quincy.white@example.com",
                    SourceSystem = "AzureAD",
                    LastUpdated = DateTime.Now,
                    IsActive = true
                }
        };

        context.UserIdentities.AddRange(userIdentities);
        context.SaveChanges();
    }
}
