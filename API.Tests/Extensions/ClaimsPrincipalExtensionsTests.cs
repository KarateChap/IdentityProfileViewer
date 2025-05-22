using System.Collections.Generic;
using System.Security.Claims;
using API.Extensions;
using Xunit;

namespace API.Tests.Extensions
{
    public class ClaimsPrincipalExtensionsTests
    {
        [Fact]
        public void GetUserId_WithValidClaimsPrincipal_ReturnsUserId()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            
            // Act
            var userId = claimsPrincipal.GetUserId();
            
            // Assert
            Assert.Equal(1, userId);
        }
        
        [Fact]
        public void GetUserId_WithoutNameIdentifierClaim_ThrowsException()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, "test@example.com")
                // No NameIdentifier claim
            };
            
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            
            // Act & Assert
            Assert.Throws<Exception>(() => claimsPrincipal.GetUserId());
        }
        
        [Fact]
        public void GetUserId_WithEmptyClaimsPrincipal_ThrowsException()
        {
            // Arrange
            var claimsPrincipal = new ClaimsPrincipal();
            
            // Act & Assert
            Assert.Throws<Exception>(() => claimsPrincipal.GetUserId());
        }
    }
}
