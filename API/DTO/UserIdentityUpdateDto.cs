using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTO;

public class UserIdentityUpdateDto
{
    [StringLength(100)]
    public string? FullName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    public bool? IsActive { get; set; }
}