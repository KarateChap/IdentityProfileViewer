using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Models;
using API.Services;
using API.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace API.Tests.Services
{
    public class TokenServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<UserManager<AppUser>> _mockUserManager;
        private readonly TokenService _tokenService;
        private readonly int _testUserId = 123;

        public TokenServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(x => x["TokenKey"]).Returns("super_secret_key_that_is_at_least_64_characters_long_for_testing_purposes_only");

            // Use MockHelpers instead of directly mocking UserManager
            _mockUserManager = MockHelpers.TestUserManager<AppUser>();
            
            _tokenService = new TokenService(_mockConfiguration.Object, _mockUserManager.Object);
        }

        [Fact]
        public async Task CreateToken_ValidUser_ReturnsValidToken()
        {
            // Arrange
            var user = new AppUser
            {
                Id = _testUserId,
                UserName = "testuser",
                Email = "test@example.com"
            };

            _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<AppUser>()))
                .ReturnsAsync(new List<string> { "User" });

            // Act
            var token = await _tokenService.CreateToken(user);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);

            // Validate token structure
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Check claims - use constant strings instead of ClaimTypes to match actual JWT format
            var claims = jwtToken.Claims;
            Assert.Contains(claims, c => c.Type == "nameid" && c.Value == _testUserId.ToString());
            Assert.Contains(claims, c => c.Type == "email" && c.Value == "test@example.com");
            Assert.Contains(claims, c => c.Type == "role" && c.Value == "User");

            // Check expiration
            Assert.True(jwtToken.ValidTo > DateTime.UtcNow);
            Assert.True(jwtToken.ValidTo <= DateTime.UtcNow.AddMinutes(16)); // Should be around 15 Minutes
        }

        [Fact]
        public void GenerateRefreshToken_ReturnsValidRefreshToken()
        {
            // Act
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Assert
            Assert.NotNull(refreshToken);
            Assert.NotNull(refreshToken.Token);
            Assert.NotEmpty(refreshToken.Token);
            Assert.Equal(DateTime.UtcNow.AddDays(7).Date, refreshToken.Expires.Date);
            Assert.Null(refreshToken.Revoked);
            Assert.True(refreshToken.IsActive);
        }

        [Fact]
        public async Task CreateToken_UserWithoutEmail_ThrowsException()
        {
            // Arrange
            var user = new AppUser
            {
                Id = 1,
                UserName = "testuser",
                Email = null
            };

            _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<AppUser>()))
                .ReturnsAsync(new List<string> { "User" });

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _tokenService.CreateToken(user));
        }

        [Fact]
        public async Task CreateToken_WithMultipleRoles_IncludesAllRolesInToken()
        {
            // Arrange
            var user = new AppUser
            {
                Id = _testUserId,
                UserName = "testuser",
                Email = "test@example.com"
            };

            _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<AppUser>()))
                .ReturnsAsync(new List<string> { "User", "Admin", "Moderator" });

            // Act
            var token = await _tokenService.CreateToken(user);

            // Assert
            Assert.NotNull(token);

            // Validate token structure
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Check role claims - use constant strings to match actual JWT format
            var claims = jwtToken.Claims;
            Assert.Contains(claims, c => c.Type == "role" && c.Value == "User");
            Assert.Contains(claims, c => c.Type == "role" && c.Value == "Admin");
            Assert.Contains(claims, c => c.Type == "role" && c.Value == "Moderator");
        }
    }
}
