using System;
using API.Data;
using API.DTO;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class UserIdentityService : IUserIdentityService
{
    private readonly UserIdentityContext _context;

    public UserIdentityService(UserIdentityContext context)
    {
        _context = context;
    }

    public async Task<UserIdentity?> GetUserIdentityAsync(int id)
    {
        return await _context.UserIdentities.FindAsync(id);
    }

    public async Task<IEnumerable<UserIdentity>> GetAllUserIdentitiesAsync()
    {
        return await _context.UserIdentities.ToListAsync();
    }

    public async Task<UserIdentity?> UpdateUserIdentityAsync(int id, UserIdentityUpdateDto updateDto)
    {
        var userIdentity = await _context.UserIdentities.FindAsync(id);
        if (userIdentity == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(updateDto.FullName))
        {
            userIdentity.FullName = updateDto.FullName;
        }

        if (!string.IsNullOrEmpty(updateDto.Email))
        {
            userIdentity.Email = updateDto.Email;
        }

        if (updateDto.IsActive.HasValue)
        {
            userIdentity.IsActive = updateDto.IsActive.Value;
        }

        userIdentity.LastUpdated = DateTime.Now;

        try
        {
            await _context.SaveChangesAsync();
            return userIdentity;
        }
        catch (DbUpdateException)
        {
            throw;
        }
    }
}
