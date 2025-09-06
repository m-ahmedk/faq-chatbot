using FluentValidation;
using FAQDemo.API.DTOs.Auth;

namespace FAQDemo.API.Validators.Auth
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator() { 
            RuleFor(x => x.Email)
                .Must(email => !string.IsNullOrWhiteSpace(email)).WithMessage("Email is required")
                .EmailAddress().WithMessage("Should match an email format");

            RuleFor(x => x.Password)
                .Must(pass => !string.IsNullOrWhiteSpace(pass)).WithMessage("Please enter your password")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
        }
    }
}
