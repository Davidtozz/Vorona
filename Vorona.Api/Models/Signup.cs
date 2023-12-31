namespace Vorona.Api.Models;

public class Signup
{
    public required string Username { get; set; } 
    public required string Email { get; set; }
    public required string Password { get; set; }
}