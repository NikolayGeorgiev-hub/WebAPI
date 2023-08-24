using Application.Services.Models.Users;
using FluentValidation;

namespace Application.Services.Validators.Users;

public class UserRegistrationRequestValidator : AbstractValidator<UserRegistrationRequestModel>
{
    public UserRegistrationRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("{PropertyName} is required");

        RuleFor(x => x.Email)
            .Must(ValidatorHelper.ValidateEmailAddress)
            .When(x => x.Email is not null)
            .WithMessage("Invalid email address");

        RuleFor(x => x.Email)
            .Length(5, 100)
            .WithMessage("{PropertyName} must by between {MinLength} - {MaxLength} symbols");

        RuleFor(x => x.Password)
           .NotEmpty()
           .WithMessage("{PropertyName} is required");

        RuleFor(x => x.Password)
            .Length(6, 100)
            .When(x => x.Password is not null)
            .WithMessage("{PropertyName} must by between {MinLength} - {MaxLength} symbols");

        RuleFor(x => x.ConfirmPassword)
           .NotEmpty()
           .WithMessage("{PropertyName} is required");

        RuleFor(x => x.ConfirmPassword)
            .Length(6, 100).When(x => x.Password is not null)
            .WithMessage("{PropertyName} must by between {MinLength} - {MaxLength} symbols");

        RuleFor(x => x)
            .Must(EqualsPasswords)
            .WithMessage("Password not match confirm password field");
    }

    private bool EqualsPasswords(UserRegistrationRequestModel requestModel)
    {
        if (!requestModel.Password.Equals(requestModel.ConfirmPassword))
        {
            return false;
        }

        return true;
    }
}
