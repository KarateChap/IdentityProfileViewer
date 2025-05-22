using System;
using API.DTO;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserIdentitiesController : ControllerBase
{
    private readonly IUserIdentityService _userIdentityService;

    public UserIdentitiesController(IUserIdentityService userIdentityService)
    {
        _userIdentityService = userIdentityService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserIdentity>>> GetUserIdentities()
    {
        var userIdentities = await _userIdentityService.GetAllUserIdentitiesAsync();
        return Ok(userIdentities);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserIdentity>> GetUserIdentity(int id)
    {
        var userIdentity = await _userIdentityService.GetUserIdentityAsync(id);

        if (userIdentity == null)
        {
            return NotFound($"User identity with ID {id} not found.");
        }

        return Ok(userIdentity);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<UserIdentity>> UpdateUserIdentity(int id, UserIdentityUpdateDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updatedUserIdentity = await _userIdentityService.UpdateUserIdentityAsync(id, updateDto);

            if (updatedUserIdentity == null)
            {
                return NotFound($"User identity with ID {id} not found.");
            }

            return Ok(updatedUserIdentity);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the user identity: {ex.Message}");
        }
    }
}
