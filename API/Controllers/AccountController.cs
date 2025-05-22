using System;
using System.Text;
using API.DTO;
using API.Extensions;
using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
    ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(UserAuthDto userAuthDto)
    {
        if (await UserExists(userAuthDto.Email)) return BadRequest("Email is already used");

        var user = new AppUser
        {
            UserName = userAuthDto.Email.ToLower(),
            Email = userAuthDto.Email.ToLower()
        };

        var result = await userManager.CreateAsync(user, userAuthDto.Password);
        if (!result.Succeeded) return BadRequest("Problem registering user");

        await userManager.AddToRoleAsync(user, "User");

        return Ok(new { message = "Registration success" });
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(UserAuthDto userAuthDto)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(x => x.NormalizedEmail == userAuthDto.Email.ToUpper());

        if (user == null || user.Email == null) return Unauthorized("Invalid email");

        var result = await signInManager.CheckPasswordSignInAsync(user, userAuthDto.Password, false);

        if (!result.Succeeded) return Unauthorized("Invalid password");

        await SetRefreshToken(user);

        var userToReturn = new UserDto
        {
            Id = user.Id,
            Token = "",
            Email = user.Email!
        };

        userToReturn.Token = await tokenService.CreateToken(user);

        return userToReturn;
    }

    [Authorize]
    [HttpPost("refreshToken")]
    public async Task<ActionResult<UserDto>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var user = await userManager.Users.Include(x => x.RefreshTokens)
            .FirstOrDefaultAsync(x => x.Id == User.GetUserId());

        if (user == null) return Unauthorized();

        var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);

        if (oldToken != null && !oldToken.IsActive) return Unauthorized();

        if (oldToken != null) oldToken.Revoked = DateTime.UtcNow;

        var userToReturn = new UserDto
        {
            Id = user.Id,
            Token = "",
            Email = user.Email!
        };

        userToReturn.Token = await tokenService.CreateToken(user);

        return userToReturn;
    }

    private async Task SetRefreshToken(AppUser user)
    {
        var refreshToken = tokenService.GenerateRefreshToken();

        user.RefreshTokens.Add(refreshToken);
        await userManager.UpdateAsync(user);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
    }

    private async Task<bool> UserExists(string email)
    {
        return await userManager.Users.AnyAsync(x => x.NormalizedEmail == email.ToUpper());
    }
}