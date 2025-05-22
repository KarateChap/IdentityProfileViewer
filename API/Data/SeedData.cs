using System;
using API.Models;

namespace API.Data;

public static class SeedData
{
    public static void Initialize(UserIdentityContext context)
    {
        if (context.UserIdentities.Any())
        {
            return;
        }

        var userIdentities = new UserIdentity[]
        {
                new UserIdentity
                {
                    UserId = "1",
                    FullName = "Ralph Gabriel Mariano",
                    Email = "gabriel.mariano@example.com",
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
                }
        };

        context.UserIdentities.AddRange(userIdentities);
        context.SaveChanges();
    }
}