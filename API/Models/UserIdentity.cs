using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models;

public class UserIdentity
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string SourceSystem { get; set; } = string.Empty;

    public DateTime LastUpdated { get; set; }

    public bool IsActive { get; set; }
}
