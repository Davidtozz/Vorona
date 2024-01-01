namespace Vorona.Api.Models;

public class SignupModel
{
    public required string Username { get; set; } 
    public required string Email { get; set; }
    public required string Password { get; set; }
}