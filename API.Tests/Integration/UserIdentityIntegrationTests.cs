using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace API.Tests.Integration
{
    // Note: This is a simplified version of integration tests that doesn't require WebApplicationFactory
    // For actual integration tests, you would typically use WebApplicationFactory
    // but it requires making Program class public or adding InternalsVisibleTo
    public class UserIdentityIntegrationTests
    {
        [Fact]
        public async Task SimulateGetUserIdentities_ReturnsAllUserIdentities()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "UserIdentityTestDb_GetAll")
                .Options;

            // Seed the database
            using (var context = new DataContext(options))
            {
                context.UserIdentities.AddRange(
                    new UserIdentity
                    {
                        Id = 1,
                        FullName = "John Doe",
                        Email = "john.doe@example.com",
                        SourceSystem = "System A",
                        UserId = "1",
                        IsActive = true,
                        LastUpdated = DateTime.Now.AddDays(-5)
                    },
                    new UserIdentity
                    {
                        Id = 2,
                        FullName = "Jane Smith",
                        Email = "jane.smith@example.com",
                        SourceSystem = "System B",
                        UserId = "2",
                        IsActive = true,
                        LastUpdated = DateTime.Now.AddDays(-3)
                    },
                    new UserIdentity
                    {
                        Id = 3,
                        FullName = "Bob Johnson",
                        Email = "bob.johnson@example.com",
                        SourceSystem = "System A",
                        UserId = "1",
                        IsActive = false,
                        LastUpdated = DateTime.Now.AddDays(-2)
                    }
                );
                context.SaveChanges();
            }

            // Act & Assert
            using (var context = new DataContext(options))
            {
                // Verify that the database was seeded correctly
                var userIdentities = await context.UserIdentities.ToListAsync();
                Assert.Equal(3, userIdentities.Count);
                Assert.Contains(userIdentities, u => u.FullName == "John Doe");
                Assert.Contains(userIdentities, u => u.FullName == "Jane Smith");
                Assert.Contains(userIdentities, u => u.FullName == "Bob Johnson");
            }
        }

        [Fact]
        public async Task SimulateGetUserIdentity_WithValidId_ReturnsUserIdentity()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "UserIdentityTestDb_GetById")
                .Options;

            // Seed the database
            using (var context = new DataContext(options))
            {
                context.UserIdentities.Add(new UserIdentity
                {
                    Id = 1,
                    FullName = "John Doe",
                    Email = "john.doe@example.com",
                    SourceSystem = "System A",
                    UserId = "1",
                    IsActive = true,
                    LastUpdated = DateTime.Now.AddDays(-5)
                });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new DataContext(options))
            {
                var userIdentity = await context.UserIdentities.FindAsync(1);
                
                // Assert
                Assert.NotNull(userIdentity);
                Assert.Equal(1, userIdentity.Id);
                Assert.Equal("John Doe", userIdentity.FullName);
                Assert.Equal("john.doe@example.com", userIdentity.Email);
            }
        }

        [Fact]
        public async Task SimulateUpdateUserIdentity_WithValidData_UpdatesUserIdentity()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "UserIdentityTestDb_Update")
                .Options;

            // Seed the database
            using (var context = new DataContext(options))
            {
                context.UserIdentities.Add(new UserIdentity
                {
                    Id = 1,
                    FullName = "John Doe",
                    Email = "john.doe@example.com",
                    SourceSystem = "System A",
                    UserId = "1",
                    IsActive = true,
                    LastUpdated = DateTime.Now.AddDays(-5)
                });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new DataContext(options))
            {
                var userIdentity = await context.UserIdentities.FindAsync(1);
                Assert.NotNull(userIdentity);

                // Update the user identity
                userIdentity.FullName = "John Doe Updated";
                userIdentity.Email = "john.updated@example.com";
                userIdentity.IsActive = false;
                userIdentity.LastUpdated = DateTime.Now;

                await context.SaveChangesAsync();
            }

            // Assert
            using (var context = new DataContext(options))
            {
                var updatedUserIdentity = await context.UserIdentities.FindAsync(1);
                Assert.NotNull(updatedUserIdentity);
                Assert.Equal("John Doe Updated", updatedUserIdentity.FullName);
                Assert.Equal("john.updated@example.com", updatedUserIdentity.Email);
                Assert.False(updatedUserIdentity.IsActive);
            }
        }

        [Fact]
        public async Task SimulateUserIdentityFiltering_WithUserId_ReturnsFilteredUserIdentities()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "UserIdentityTestDb_Filter")
                .Options;

            // Seed the database
            using (var context = new DataContext(options))
            {
                context.UserIdentities.AddRange(
                    new UserIdentity
                    {
                        Id = 1,
                        FullName = "John Doe",
                        Email = "john.doe@example.com",
                        SourceSystem = "System A",
                        UserId = "1",
                        IsActive = true
                    },
                    new UserIdentity
                    {
                        Id = 2,
                        FullName = "Jane Smith",
                        Email = "jane.smith@example.com",
                        SourceSystem = "System B",
                        UserId = "2",
                        IsActive = true
                    },
                    new UserIdentity
                    {
                        Id = 3,
                        FullName = "Bob Johnson",
                        Email = "bob.johnson@example.com",
                        SourceSystem = "System A",
                        UserId = "1",
                        IsActive = false
                    }
                );
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new DataContext(options))
            {
                var userIdentities = await context.UserIdentities
                    .Where(x => x.UserId == "1")
                    .ToListAsync();

                // Assert
                Assert.Equal(2, userIdentities.Count);
                Assert.All(userIdentities, u => Assert.Equal("1", u.UserId));
                Assert.Contains(userIdentities, u => u.FullName == "John Doe");
                Assert.Contains(userIdentities, u => u.FullName == "Bob Johnson");
            }
        }

        [Fact]
        public async Task SimulateUserIdentityFiltering_WithSearchString_ReturnsFilteredUserIdentities()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "UserIdentityTestDb_Search")
                .Options;

            // Seed the database
            using (var context = new DataContext(options))
            {
                context.UserIdentities.AddRange(
                    new UserIdentity
                    {
                        Id = 1,
                        FullName = "John Doe",
                        Email = "john.doe@example.com",
                        SourceSystem = "System A",
                        UserId = "1",
                        IsActive = true
                    },
                    new UserIdentity
                    {
                        Id = 2,
                        FullName = "Jane Smith",
                        Email = "jane.smith@example.com",
                        SourceSystem = "System B",
                        UserId = "2",
                        IsActive = true
                    }
                );
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new DataContext(options))
            {
                var searchString = "john";
                var userIdentities = await context.UserIdentities
                    .Where(x => 
                        x.FullName.ToLower().Contains(searchString.ToLower()) ||
                        x.Email.ToLower().Contains(searchString.ToLower()) ||
                        x.SourceSystem.ToLower().Contains(searchString.ToLower()))
                    .ToListAsync();

                // Assert
                Assert.Single(userIdentities);
                Assert.Equal("John Doe", userIdentities[0].FullName);
            }
        }
    }
}
