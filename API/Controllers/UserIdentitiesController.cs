using System;
using API.DTO;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class IdentitiesController : BaseApiController
{
    private readonly IUserIdentityService _userIdentityService;

    public IdentitiesController(IUserIdentityService userIdentityService)
    {
        _userIdentityService = userIdentityService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserIdentity>>> GetUserIdentities([FromQuery] UserIdentityParams userIdentityParams)
    {
        var userIdentities = await _userIdentityService.GetAllUserIdentitiesAsync(userIdentityParams);

        Response.AddPaginationHeader(userIdentities);

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
        if (id <= 0)
            return BadRequest("Invalid user ID.");

        if (updateDto == null)
            return BadRequest("Update data is required.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updatedUserIdentity = await _userIdentityService.UpdateUserIdentityAsync(id, updateDto);

            if (updatedUserIdentity == null)
                return NotFound($"User identity with ID {id} not found.");

            return Ok(updatedUserIdentity);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"User identity with ID {id} not found.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while updating the user identity.");
        }
    }
}
