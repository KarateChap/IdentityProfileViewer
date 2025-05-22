using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Helpers;
using API.Models;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace API.Tests.Services
{
    public class UserIdentityServiceTests
    {
        private readonly DbContextOptions<DataContext> _options;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        
        public UserIdentityServiceTests()
        {
            _options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            SetupHttpContext();
        }
        
        private void SetupHttpContext(string userId = "1")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            
            var httpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            };
            
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
        }
        
        private async Task SeedDatabase()
        {
            using var context = new DataContext(_options);
            
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
            
            await context.SaveChangesAsync();
        }
        
        [Fact]
        public async Task GetUserIdentityAsync_WithValidId_ReturnsUserIdentity()
        {
            // Arrange
            await SeedDatabase();
            
            using var context = new DataContext(_options);
            var service = new UserIdentityService(context, _mockHttpContextAccessor.Object);
            
            // Act
            var result = await service.GetUserIdentityAsync(1);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("John Doe", result.FullName);
            Assert.Equal("john.doe@example.com", result.Email);
        }
        
        [Fact]
        public async Task GetUserIdentityAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            await SeedDatabase();
            
            using var context = new DataContext(_options);
            var service = new UserIdentityService(context, _mockHttpContextAccessor.Object);
            
            // Act
            var result = await service.GetUserIdentityAsync(999);
            
            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task GetAllUserIdentitiesAsync_WithNoFilters_ReturnsAllUserIdentities()
        {
            // Arrange
            await SeedDatabase();
            
            using var context = new DataContext(_options);
            var service = new UserIdentityService(context, _mockHttpContextAccessor.Object);
            
            var userIdentityParams = new UserIdentityParams
            {
                PageNumber = 1,
                PageSize = 10
            };
            
            // Act
            var result = await service.GetAllUserIdentitiesAsync(userIdentityParams);
            
            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(1, result.TotalPages);
        }
        
        [Fact]
        public async Task GetAllUserIdentitiesAsync_WithUserIdFilter_ReturnsFilteredUserIdentities()
        {
            // Arrange
            await SeedDatabase();
            
            using var context = new DataContext(_options);
            var service = new UserIdentityService(context, _mockHttpContextAccessor.Object);
            
            var userIdentityParams = new UserIdentityParams
            {
                PageNumber = 1,
                PageSize = 10,
                UserId = "1"
            };
            
            // Act
            var result = await service.GetAllUserIdentitiesAsync(userIdentityParams);
            
            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(1, result.TotalPages);
            Assert.All(result, item => Assert.Equal("1", item.UserId));
        }
        
        [Fact]
        public async Task GetAllUserIdentitiesAsync_WithSearchString_ReturnsFilteredUserIdentities()
        {
            // Arrange
            await SeedDatabase();
            
            using var context = new DataContext(_options);
            var service = new UserIdentityService(context, _mockHttpContextAccessor.Object);
            
            var userIdentityParams = new UserIdentityParams
            {
                PageNumber = 1,
                PageSize = 10,
                SearchString = "John Doe"
            };
            
            // Act
            var result = await service.GetAllUserIdentitiesAsync(userIdentityParams);
            
            // Assert
            Assert.Single(result);
            Assert.Equal(1, result.TotalCount);
            Assert.Equal("John Doe", result[0].FullName);
        }
        
        [Fact]
        public async Task UpdateUserIdentityAsync_WithValidIdAndData_UpdatesUserIdentity()
        {
            // Arrange
            await SeedDatabase();
            
            using var context = new DataContext(_options);
            var service = new UserIdentityService(context, _mockHttpContextAccessor.Object);
            
            var updateDto = new UserIdentityUpdateDto
            {
                FullName = "John Doe Updated",
                Email = "john.updated@example.com",
                IsActive = false
            };
            
            // Act
            var result = await service.UpdateUserIdentityAsync(1, updateDto);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("John Doe Updated", result.FullName);
            Assert.Equal("john.updated@example.com", result.Email);
            Assert.False(result.IsActive);
            
            // Verify the changes were persisted
            var updatedEntity = await context.UserIdentities.FindAsync(1);
            Assert.NotNull(updatedEntity);
            Assert.Equal("John Doe Updated", updatedEntity!.FullName);
            Assert.Equal("john.updated@example.com", updatedEntity.Email);
            Assert.False(updatedEntity.IsActive);
        }
        
        [Fact]
        public async Task UpdateUserIdentityAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            await SeedDatabase();
            
            using var context = new DataContext(_options);
            var service = new UserIdentityService(context, _mockHttpContextAccessor.Object);
            
            var updateDto = new UserIdentityUpdateDto
            {
                FullName = "John Doe Updated",
                Email = "john.updated@example.com",
                IsActive = false
            };
            
            // Act
            var result = await service.UpdateUserIdentityAsync(999, updateDto);
            
            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task UpdateUserIdentityAsync_WithPartialData_OnlyUpdatesProvidedFields()
        {
            // Arrange
            await SeedDatabase();
            
            using var context = new DataContext(_options);
            var service = new UserIdentityService(context, _mockHttpContextAccessor.Object);
            
            var originalEntity = await context.UserIdentities.FindAsync(1);
            var originalEmail = originalEntity.Email;
            var originalIsActive = originalEntity.IsActive;
            
            var updateDto = new UserIdentityUpdateDto
            {
                FullName = "John Doe Updated"
                // Email and IsActive not provided
            };
            
            // Act
            var result = await service.UpdateUserIdentityAsync(1, updateDto);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("John Doe Updated", result.FullName);
            Assert.Equal(originalEmail, result.Email); // Should remain unchanged
            Assert.Equal(originalIsActive, result.IsActive); // Should remain unchanged
            
            // Verify the changes were persisted
            var updatedEntity = await context.UserIdentities.FindAsync(1);
            Assert.NotNull(updatedEntity);
            Assert.Equal("John Doe Updated", updatedEntity!.FullName);
            Assert.Equal(originalEmail, updatedEntity.Email);
            Assert.Equal(originalIsActive, updatedEntity.IsActive);
        }
    }
}
