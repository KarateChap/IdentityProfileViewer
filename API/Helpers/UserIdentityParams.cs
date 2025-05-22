using System;

namespace API.Helpers;

public class UserIdentityParams : PaginationParams
{
    public string? UserId { get; set; }
    public string? SearchString { get; set; }
}
