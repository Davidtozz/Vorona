using FluentValidation;

namespace Vorona.Api.Models;

public class LoginModel
{
    public required string Username { get; set; } 
    public required string Password { get; set; } 
}

public class LoginValidator : AbstractValidator<LoginModel>
{
    public LoginValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty(); 
    }
}