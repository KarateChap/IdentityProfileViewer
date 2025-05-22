using System.Collections.Generic;
using System.Text.Json;
using API.Extensions;
using API.Helpers;
using API.Models;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace API.Tests.Extensions
{
    public class HttpExtensionsTests
    {
        [Fact]
        public void AddPaginationHeader_AddsCorrectHeaderToResponse()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var response = httpContext.Response;
            
            var items = new List<UserIdentity>
            {
                new UserIdentity { Id = 1, FullName = "User 1", Email = "user1@example.com", UserId = "1", SourceSystem = "System A", IsActive = true },
                new UserIdentity { Id = 2, FullName = "User 2", Email = "user2@example.com", UserId = "2", SourceSystem = "System B", IsActive = true },
            };
            
            // Create a PagedList with specific values for testing
            var pagedList = new PagedList<UserIdentity>(items, 10, 2, 5);
            
            // Create a manual pagination header (bypassing the actual implementation)
            var manualHeader = new PaginationHeader(2, 5, 10, 2);
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var expectedJson = JsonSerializer.Serialize(manualHeader, jsonOptions);
            
            // Act
            response.AddPaginationHeader(pagedList);
            
            // Assert
            Assert.True(response.Headers.ContainsKey("Pagination"));
            
            var paginationHeader = response.Headers["Pagination"].ToString();
            // Replace this test with a direct string comparison
            Assert.Equal(expectedJson, paginationHeader);
            
            // Also test the deserialized object for additional verification
            var paginationObject = JsonSerializer.Deserialize<PaginationHeader>(paginationHeader, jsonOptions);
            
            Assert.NotNull(paginationObject);
            Assert.Equal(2, paginationObject.CurrentPage);
            Assert.Equal(5, paginationObject.ItemsPerPage);
            Assert.Equal(10, paginationObject.TotalItems);
            // Test against our manually set value
            Assert.Equal(2, paginationObject.TotalPages);
        }
        
        [Fact]
        public void AddPaginationHeader_SetsCorrectAccessControlExposeHeaders()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var response = httpContext.Response;
            
            var items = new List<UserIdentity>
            {
                new UserIdentity { Id = 1, FullName = "User 1", Email = "user1@example.com", UserId = "1", SourceSystem = "System A", IsActive = true },
                new UserIdentity { Id = 2, FullName = "User 2", Email = "user2@example.com", UserId = "2", SourceSystem = "System B", IsActive = true },
            };
            
            var pagedList = new PagedList<UserIdentity>(items, 10, 2, 5);
            
            // Act
            response.AddPaginationHeader(pagedList);
            
            // Assert
            Assert.True(response.Headers.ContainsKey("Access-Control-Expose-Headers"));
            Assert.Equal("Pagination", response.Headers["Access-Control-Expose-Headers"]);
        }
    }
}
