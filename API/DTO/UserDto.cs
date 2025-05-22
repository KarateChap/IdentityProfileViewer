using System;

namespace API.DTO;

public class UserDto
{
    public int Id { get; set; }
    public required string Token { get; set; }
    public required string Email { get; set; }
}
