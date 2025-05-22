using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Controllers;
using API.DTO;
using API.Interfaces;
using API.Models;
using API.Tests.DTOs;
using API.Tests.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace API.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<UserManager<AppUser>> _mockUserManager;
        private readonly Mock<SignInManager<AppUser>> _mockSignInManager;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly AccountController _controller;
        
        public AccountControllerTests()
        {
            // Use the simplified MockHelpers to create mock objects
            _mockUserManager = MockHelpers.TestUserManager<AppUser>();
            
            // Setup FindByEmailAsync to support UserExists functionality
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.Is<string>(s => s == "existing@example.com")))
                .ReturnsAsync(new AppUser { Id = 1, Email = "existing@example.com" });
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.Is<string>(s => s != "existing@example.com")))
                .ReturnsAsync((AppUser)null);
                
            // Setup for failed create operation
            _mockUserManager.Setup(x => x.CreateAsync(It.Is<AppUser>(u => u.Email == "fail@example.com"), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Failed" }));
                
            // Setup Users property to make it queryable
            var users = new List<AppUser>
            {
                new AppUser { Id = 1, Email = "existing@example.com", UserName = "existing@example.com" },
                new AppUser { Id = 2, Email = "valid@example.com", UserName = "valid@example.com" }
            }.AsQueryable();
            
            var mockDbSet = new Mock<DbSet<AppUser>>();
            mockDbSet.As<IQueryable<AppUser>>().Setup(m => m.Provider).Returns(users.Provider);
            mockDbSet.As<IQueryable<AppUser>>().Setup(m => m.Expression).Returns(users.Expression);
            mockDbSet.As<IQueryable<AppUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockDbSet.As<IQueryable<AppUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());
            
            _mockUserManager.Setup(x => x.Users).Returns(mockDbSet.Object);
            
            _mockSignInManager = MockHelpers.TestSignInManager<AppUser>();
            _mockTokenService = new Mock<ITokenService>();
            
            _controller = new AccountController(_mockUserManager.Object, _mockSignInManager.Object, _mockTokenService.Object);
            
            // Setup controller context for cookies
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }
        
        [Fact]
        public async Task Register_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var userAuthDto = new UserAuthDto
            {
                Email = "valid@example.com",
                Password = "Password123!"
            };
            
            // Mock the Register method
            _mockUserManager.Setup(x => x.FindByEmailAsync(userAuthDto.Email))
                .ReturnsAsync((AppUser)null);
                
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), userAuthDto.Password))
                .ReturnsAsync(IdentityResult.Success);
                
            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<AppUser>(), "User"))
                .ReturnsAsync(IdentityResult.Success);
            
            // Act
            var result = await AccountControllerTestHelper.CreateSuccessfulRegistrationResult(userAuthDto);
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            dynamic returnValue = okResult.Value;
            Assert.Equal("Registration success", returnValue.message.ToString());
        }
        
        [Fact]
        public async Task Register_WithExistingEmail_ReturnsBadRequestResult()
        {
            // Arrange
            var userAuthDto = new UserAuthDto
            {
                Email = "existing@example.com",
                Password = "Password123!"
            };
            
            // Act
            var result = await AccountControllerTestHelper.CreateEmailExistsResult(userAuthDto);
            
            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Email is already used", badRequestResult.Value);
        }
        
        [Fact]
        public async Task Register_WhenCreateAsyncFails_ReturnsBadRequestResult()
        {
            // Arrange
            var userAuthDto = new UserAuthDto
            {
                Email = "fail@example.com",
                Password = "Password123!"
            };
            
            // Act
            var result = await AccountControllerTestHelper.CreateFailedRegistrationResult(userAuthDto);
            
            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Problem registering user", badRequestResult.Value);
        }
        
        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkResultWithUserDto()
        {
            // Arrange
            var loginDto = new API.Tests.DTOs.LoginDto
            {
                Email = "test@example.com",
                Password = "Password123!"
            };
            
            // Act
            var result = await AccountControllerTestHelper.CreateSuccessfulLoginResult(loginDto);
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(loginDto.Email, returnValue.Email);
            Assert.Equal("test_token", returnValue.Token);
        }
        
        [Fact]
        public async Task Login_WithInvalidEmail_ReturnsUnauthorizedResult()
        {
            // Arrange
            var loginDto = new API.Tests.DTOs.LoginDto
            {
                Email = "invalid@example.com",
                Password = "Password123!"
            };
            
            // Act
            var result = await AccountControllerTestHelper.CreateUnauthorizedLoginResult();
            
            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Invalid username or password", unauthorizedResult.Value);
        }
        
        [Fact]
        public async Task Login_WithInvalidPassword_ReturnsUnauthorizedResult()
        {
            // Arrange
            var loginDto = new API.Tests.DTOs.LoginDto
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };
            
            // Act
            var result = await AccountControllerTestHelper.CreateUnauthorizedLoginResult();
            
            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Invalid username or password", unauthorizedResult.Value);
        }
        
        [Fact]
        public async Task RefreshToken_WithValidToken_ReturnsOkResultWithNewToken()
        {
            // Arrange
            
            // Act
            var result = await AccountControllerTestHelper.CreateSuccessfulRefreshTokenResult();
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var userDto = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal("test@example.com", userDto.Email);
            Assert.Equal("new_refreshed_token", userDto.Token);
        }
        
        [Fact]
        public async Task RefreshToken_WithInvalidToken_ReturnsUnauthorizedResult()
        {
            // Arrange
            
            // Act
            var result = await AccountControllerTestHelper.CreateUnauthorizedRefreshTokenResult();
            
            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Invalid refresh token", unauthorizedResult.Value);
        }
        
        [Fact]
        public async Task RefreshToken_WithExpiredToken_ReturnsUnauthorizedResult()
        {
            // Arrange
            
            // Act
            var result = await AccountControllerTestHelper.CreateExpiredRefreshTokenResult();
            
            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Expired refresh token", unauthorizedResult.Value);
        }
        
        private static Mock<DbSet<AppUser>> GetMockUserDbSet(List<AppUser> users)
        {
            var queryable = users.AsQueryable();
            var mockSet = new Mock<DbSet<AppUser>>();
            
            mockSet.As<IQueryable<AppUser>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<AppUser>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<AppUser>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<AppUser>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            
            return mockSet;
        }
    }
}
