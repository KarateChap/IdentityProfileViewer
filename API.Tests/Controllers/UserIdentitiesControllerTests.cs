using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Controllers;
using API.DTO;
using API.Helpers;
using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace API.Tests.Controllers
{
    public class UserIdentitiesControllerTests
    {
        private readonly Mock<IUserIdentityService> _mockUserIdentityService;
        private readonly IdentitiesController _controller;
        
        public UserIdentitiesControllerTests()
        {
            _mockUserIdentityService = new Mock<IUserIdentityService>();
            _controller = new IdentitiesController(_mockUserIdentityService.Object);
            
            // Setup controller context for pagination headers
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }
        
        [Fact]
        public async Task GetUserIdentities_ReturnsOkResultWithUserIdentities()
        {
            // Arrange
            var userIdentityParams = new UserIdentityParams();
            var pagedList = GetMockPagedList();
            
            _mockUserIdentityService.Setup(service => service.GetAllUserIdentitiesAsync(It.IsAny<UserIdentityParams>()))
                .ReturnsAsync(pagedList);
            
            // Act
            var result = await _controller.GetUserIdentities(userIdentityParams);
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<PagedList<UserIdentity>>(okResult.Value);
            Assert.Equal(3, returnValue.Count);
        }
        
        [Fact]
        public async Task GetUserIdentity_WithValidId_ReturnsOkResultWithUserIdentity()
        {
            // Arrange
            var userIdentity = new UserIdentity
            {
                Id = 1,
                FullName = "John Doe",
                Email = "john.doe@example.com",
                SourceSystem = "System A",
                UserId = "1",
                IsActive = true
            };
            
            _mockUserIdentityService.Setup(service => service.GetUserIdentityAsync(1))
                .ReturnsAsync(userIdentity);
            
            // Act
            var result = await _controller.GetUserIdentity(1);
            
            // Assert
            Assert.NotNull(result.Result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            var returnValue = Assert.IsType<UserIdentity>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("John Doe", returnValue.FullName);
        }
        
        [Fact]
        public async Task GetUserIdentity_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            _mockUserIdentityService.Setup(service => service.GetUserIdentityAsync(999))
                .ReturnsAsync((UserIdentity)null);
            
            // Act
            var result = await _controller.GetUserIdentity(999);
            
            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }
        
        [Fact]
        public async Task UpdateUserIdentity_WithValidIdAndData_ReturnsOkResultWithUpdatedUserIdentity()
        {
            // Arrange
            var updateDto = new UserIdentityUpdateDto
            {
                FullName = "John Doe Updated",
                Email = "john.updated@example.com",
                IsActive = false
            };
            
            var updatedUserIdentity = new UserIdentity
            {
                Id = 1,
                FullName = "John Doe Updated",
                Email = "john.updated@example.com",
                IsActive = false,
                SourceSystem = "System A",
                UserId = "1"
            };
            
            _mockUserIdentityService.Setup(service => service.UpdateUserIdentityAsync(1, updateDto))
                .ReturnsAsync(updatedUserIdentity);
            
            // Act
            var result = await _controller.UpdateUserIdentity(1, updateDto);
            
            // Assert
            Assert.NotNull(result.Result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            var returnValue = Assert.IsType<UserIdentity>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("John Doe Updated", returnValue.FullName);
            Assert.Equal("john.updated@example.com", returnValue.Email);
            Assert.False(returnValue.IsActive);
        }
        
        [Fact]
        public async Task UpdateUserIdentity_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var updateDto = new UserIdentityUpdateDto
            {
                FullName = "John Doe Updated"
            };
            
            _mockUserIdentityService.Setup(service => service.UpdateUserIdentityAsync(999, updateDto))
                .ReturnsAsync((UserIdentity)null);
            
            // Act
            var result = await _controller.UpdateUserIdentity(999, updateDto);
            
            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }
        
        [Fact]
        public async Task UpdateUserIdentity_WithNegativeId_ReturnsBadRequestResult()
        {
            // Arrange
            var updateDto = new UserIdentityUpdateDto
            {
                FullName = "John Doe Updated"
            };
            
            // Act
            var result = await _controller.UpdateUserIdentity(-1, updateDto);
            
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
        
        [Fact]
        public async Task UpdateUserIdentity_WithNullUpdateDto_ReturnsBadRequestResult()
        {
            // Arrange
            UserIdentityUpdateDto? updateDto = null;
            
            // Act
            // Use a pragma to suppress the compiler warning for this specific test case
            // The test is specifically checking the behavior when a null dto is passed
            #pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type
            var result = await _controller.UpdateUserIdentity(1, updateDto);
            #pragma warning restore CS8625
            
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
        
        [Fact]
        public async Task UpdateUserIdentity_WhenServiceThrowsArgumentException_ReturnsBadRequestResult()
        {
            // Arrange
            var updateDto = new UserIdentityUpdateDto
            {
                FullName = "John Doe Updated"
            };
            
            _mockUserIdentityService.Setup(service => service.UpdateUserIdentityAsync(1, updateDto))
                .ThrowsAsync(new ArgumentException("Invalid data"));
            
            // Act
            var result = await _controller.UpdateUserIdentity(1, updateDto);
            
            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
        
        [Fact]
        public async Task UpdateUserIdentity_WhenServiceThrowsKeyNotFoundException_ReturnsNotFoundResult()
        {
            // Arrange
            var updateDto = new UserIdentityUpdateDto
            {
                FullName = "John Doe Updated"
            };
            
            _mockUserIdentityService.Setup(service => service.UpdateUserIdentityAsync(1, updateDto))
                .ThrowsAsync(new KeyNotFoundException());
            
            // Act
            var result = await _controller.UpdateUserIdentity(1, updateDto);
            
            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }
        
        [Fact]
        public async Task UpdateUserIdentity_WhenServiceThrowsGenericException_ReturnsInternalServerError()
        {
            // Arrange
            var updateDto = new UserIdentityUpdateDto
            {
                FullName = "John Doe Updated"
            };
            
            _mockUserIdentityService.Setup(service => service.UpdateUserIdentityAsync(1, updateDto))
                .ThrowsAsync(new Exception("Something went wrong"));
            
            // Act
            var result = await _controller.UpdateUserIdentity(1, updateDto);
            
            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
        
        private PagedList<UserIdentity> GetMockPagedList()
        {
            var items = new List<UserIdentity>
            {
                new UserIdentity { Id = 1, FullName = "John Doe", Email = "john.doe@example.com", SourceSystem = "System A", UserId = "1", IsActive = true },
                new UserIdentity { Id = 2, FullName = "Jane Smith", Email = "jane.smith@example.com", SourceSystem = "System B", UserId = "2", IsActive = true },
                new UserIdentity { Id = 3, FullName = "Bob Johnson", Email = "bob.johnson@example.com", SourceSystem = "System A", UserId = "1", IsActive = false }
            };
            
            // Create a PagedList with 3 items, 3 total count, page 1, and 1 total pages
            return new PagedList<UserIdentity>(items, 3, 1, 10);
        }
    }
}
