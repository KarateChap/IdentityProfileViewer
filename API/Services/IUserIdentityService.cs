using System;
using API.DTO;
using API.Models;

namespace API.Services;

public interface IUserIdentityService
{
    Task<UserIdentity?> GetUserIdentityAsync(int id);
    Task<UserIdentity?> UpdateUserIdentityAsync(int id, UserIdentityUpdateDto updateDto);
    Task<IEnumerable<UserIdentity>> GetAllUserIdentitiesAsync();
}