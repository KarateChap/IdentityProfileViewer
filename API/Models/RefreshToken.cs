using System;

namespace API.Models;

public class RefreshToken
{
    public int Id { get; set; }
    public AppUser AppUser { get; set; } = null!;
    public required string Token { get; set; }
    public DateTime Expires { get; set; } = DateTime.UtcNow.AddDays(7);
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public DateTime? Revoked { get; set; }
    public bool IsActive => Revoked == null && !IsExpired;
}
