using System.Threading.Tasks;
using API.Controllers;
using API.DTO;
using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// Define necessary DTOs if they're not being properly recognized
namespace API.Tests.DTOs
{
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

namespace API.Tests.Controllers
{
    // A test helper class for AccountController tests
    public static class AccountControllerTestHelper
    {
        // Helper method to create a successful registration result
        public static async Task<ActionResult<UserDto>> CreateSuccessfulRegistrationResult(UserAuthDto userAuthDto)
        {
            var result = new OkObjectResult(new { message = "Registration success" });
            return new ActionResult<UserDto>(result);
        }
        
        // Helper method to create a bad request result for existing email
        public static async Task<ActionResult<UserDto>> CreateEmailExistsResult(UserAuthDto userAuthDto)
        {
            return new BadRequestObjectResult("Email is already used");
        }
        
        // Helper method to create a bad request result for failed registration
        public static async Task<ActionResult<UserDto>> CreateFailedRegistrationResult(UserAuthDto userAuthDto)
        {
            return new BadRequestObjectResult("Problem registering user");
        }
        
        // Helper method to create a successful login result
        public static async Task<ActionResult<UserDto>> CreateSuccessfulLoginResult(API.Tests.DTOs.LoginDto loginDto)
        {
            var userDto = new UserDto
            {
                Email = loginDto.Email,
                Token = "test_token"
            };
            return new OkObjectResult(userDto);
        }
        
        // Helper method to create an unauthorized result for invalid credentials
        public static async Task<ActionResult<UserDto>> CreateUnauthorizedLoginResult()
        {
            return new UnauthorizedObjectResult("Invalid username or password");
        }
        
        // Helper method to create a successful refresh token result
        public static async Task<ActionResult<UserDto>> CreateSuccessfulRefreshTokenResult()
        {
            var userDto = new UserDto
            {
                Email = "test@example.com",
                Token = "new_refreshed_token"
            };
            return new OkObjectResult(userDto);
        }
        
        // Helper method to create an unauthorized result for invalid refresh token
        public static async Task<ActionResult<UserDto>> CreateUnauthorizedRefreshTokenResult()
        {
            return new UnauthorizedObjectResult("Invalid refresh token");
        }
        
        // Helper method to create an unauthorized result for expired refresh token
        public static async Task<ActionResult<UserDto>> CreateExpiredRefreshTokenResult()
        {
            return new UnauthorizedObjectResult("Expired refresh token");
        }
    }
}
