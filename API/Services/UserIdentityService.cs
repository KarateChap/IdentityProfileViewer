using System;
using API.Data;
using API.DTO;
using API.Helpers;
using API.Interfaces;
using API.Models;
using API.Extensions;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class UserIdentityService : IUserIdentityService
{
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserIdentityService(DataContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<UserIdentity?> GetUserIdentityAsync(int id)
    {
        return await _context.UserIdentities.FindAsync(id);
    }

    public async Task<PagedList<UserIdentity>> GetAllUserIdentitiesAsync(UserIdentityParams userIdentityParams)
    {
        var userId = _httpContextAccessor.HttpContext?.User.GetUserId();

        var query = _context.UserIdentities.AsQueryable();

        if (!string.IsNullOrEmpty(userIdentityParams.UserId))
        {
            query = query.Where(x => x.UserId == userIdentityParams.UserId);
        }

        if (!string.IsNullOrEmpty(userIdentityParams.SearchString) && userIdentityParams.SearchString != "all")
        {
            query = query.Where(x =>
            x.FullName.ToLower().Contains(userIdentityParams.SearchString.ToLower()) ||
            x.Email.ToLower().Contains(userIdentityParams.SearchString.ToLower()) ||
            x.SourceSystem.ToLower().Contains(userIdentityParams.SearchString.ToLower()));
        }

        return await PagedList<UserIdentity>.CreateAsync(query, userIdentityParams.PageNumber, userIdentityParams.PageSize);
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
