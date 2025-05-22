using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using API.Errors;
using API.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace API.Tests.Middleware
{
    public class ExceptionMiddlewareTests
    {
        private readonly Mock<ILogger<ExceptionMiddleware>> _mockLogger;
        private readonly Mock<IHostEnvironment> _mockEnvironment;
        private readonly ExceptionMiddleware _middleware;
        private readonly RequestDelegate _next;
        
        public ExceptionMiddlewareTests()
        {
            _mockLogger = new Mock<ILogger<ExceptionMiddleware>>();
            _mockEnvironment = new Mock<IHostEnvironment>();
            
            // Set up the next middleware in the pipeline that will throw an exception
            _next = (HttpContext context) => Task.FromException(new Exception("Test exception"));
            
            _middleware = new ExceptionMiddleware(_next, _mockLogger.Object, _mockEnvironment.Object);
        }
        
        [Fact]
        public async Task InvokeAsync_WhenExceptionIsThrown_ReturnsApiExceptionResponse()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            
            // Development environment - use EnvironmentName instead of IsDevelopment()
            _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Development");
            
            // Act
            await _middleware.InvokeAsync(context);
            
            // Assert
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(context.Response.Body);
            var responseBody = await reader.ReadToEndAsync();
            
            Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
            Assert.Contains("Test exception", responseBody);
            Assert.Contains("\"statusCode\":500", responseBody);
        }
        
        [Fact]
        public async Task InvokeAsync_WhenExceptionIsThrown_InProductionMode_DoesNotIncludeDetails()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            
            // Production environment - use EnvironmentName instead of IsDevelopment()
            _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");
            
            // Act
            await _middleware.InvokeAsync(context);
            
            // Assert
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(context.Response.Body);
            var responseBody = await reader.ReadToEndAsync();
            
            Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
            Assert.DoesNotContain("Test exception", responseBody); // Stack trace should not be included
            Assert.Contains("\"statusCode\":500", responseBody);
            Assert.Contains("\"message\":\"Internal Server Error\"", responseBody);
        }
    }
}
