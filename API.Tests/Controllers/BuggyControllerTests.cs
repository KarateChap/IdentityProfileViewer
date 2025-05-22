using System;
using API.Controllers;
using API.Data;
using API.Models;
using API.Tests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace API.Tests.Controllers
{
    public class BuggyControllerTests
    {
        private readonly BuggyController _controller;
        
        public BuggyControllerTests()
        {
            // Create mock DataContext
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "BuggyControllerTestsDb")
                .Options;
            var context = new DataContext(options);
            
            _controller = new BuggyController(context);
            
            // Setup controller context
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }
        
        [Fact]
        public void GetNotFound_ReturnsNotFoundResult()
        {
            // Act
            var result = _controller.GetNotFound();
            
            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        
        [Fact]
        public void GetBadRequest_ReturnsBadRequestResult()
        {
            // Act
            var result = _controller.GetBadRequest();
            
            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("This was not a good request", badRequestResult.Value);
        }
        
        [Fact]
        public void GetServerError_ThrowsException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _controller.GetServerError());
            
            // Assert
            Assert.Equal("A bad thing has happened", exception.Message);
        }
        
        [Fact]
        public void GetAuth_ReturnsSecretText()
        {
            // Arrange - Setup a mock user
            var userPrincipal = TestHelpers.GetClaimsPrincipal();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userPrincipal }
            };
            
            // Act
            var result = _controller.GetAuth();
            
            // Assert
            var okResult = Assert.IsType<ActionResult<string>>(result);
            Assert.Equal("secret text", okResult.Value);
        }
    }
}
