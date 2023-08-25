using Application.Services.Models.Users;
using FluentValidation;

namespace Application.Services.Validators.Users;

public class UserLoginRequestValidator : AbstractValidator<UserRequestModels.Login>
{
    public UserLoginRequestValidator()
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
    }
}
