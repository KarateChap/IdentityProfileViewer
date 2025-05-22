using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models;

public class UserIdentity
{
    public int Id { get; set; }

    [StringLength(100)]
    public required string UserId { get; set; } = string.Empty;

    [StringLength(100)]
    public required string FullName { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(100)]
    public required string Email { get; set; } = string.Empty;

    [StringLength(100)]
    public required string SourceSystem { get; set; } = string.Empty;

    public DateTime LastUpdated { get; set; }

    public bool IsActive { get; set; }
}
