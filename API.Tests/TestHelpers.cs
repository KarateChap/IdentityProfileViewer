using System.Collections.Generic;
using System.Security.Claims;

namespace API.Tests
{
    public static class TestHelpers
    {
        public static ClaimsPrincipal GetClaimsPrincipal(int userId = 1, string userName = "testuser")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, userName)
            };
            
            var identity = new ClaimsIdentity(claims);
            return new ClaimsPrincipal(identity);
        }
    }
}
