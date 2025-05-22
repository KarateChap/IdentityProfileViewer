using System;
using API.DTO;
using API.Helpers;
using API.Models;

namespace API.Interfaces;

public interface IUserIdentityService
{
    Task<UserIdentity?> GetUserIdentityAsync(int id);
    Task<UserIdentity?> UpdateUserIdentityAsync(int id, UserIdentityUpdateDto updateDto);
    Task<PagedList<UserIdentity>> GetAllUserIdentitiesAsync(UserIdentityParams userIdentityParams);
}