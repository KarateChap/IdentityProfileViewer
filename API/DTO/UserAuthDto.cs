using System;

namespace API.DTO;

public class UserAuthDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
