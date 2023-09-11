using Application.Services.Models.Users;
using FluentValidation;

namespace Application.Services.Validators.Users;

public class ChangePasswordRequestValidator : AbstractValidator<UserRequestModels.ChangePassword>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.Password)
          .NotEmpty()
          .WithMessage("{PropertyName} is required");

        RuleFor(x => x.Password)
            .Length(6, 100)
            .When(x => x.Password is not null)
            .WithMessage("{PropertyName} must by between {MinLength} - {MaxLength} symbols");

        RuleFor(x => x.NewPassword)
          .NotEmpty()
          .WithMessage("{PropertyName} is required");

        RuleFor(x => x.NewPassword)
            .Length(6, 100)
            .When(x => x.NewPassword is not null)
            .WithMessage("{PropertyName} must by between {MinLength} - {MaxLength} symbols");

        RuleFor(x => x.NewConfirmPassword)
           .NotEmpty()
           .WithMessage("{PropertyName} is required");

        RuleFor(x => x.NewConfirmPassword)
            .Length(6, 100)
            .When(x => x.NewConfirmPassword is not null)
            .WithMessage("{PropertyName} must by between {MinLength} - {MaxLength} symbols");

        RuleFor(x => x)
            .Must(EqualsPasswords)
            .When(x => x.NewPassword is not null && x.NewConfirmPassword is not null)
            .WithMessage("{PropertyName} not match confirm password field");
    }

    private bool EqualsPasswords(UserRequestModels.ChangePassword requestModel)
    {
        if (!requestModel.NewPassword.Equals(requestModel.NewConfirmPassword))
        {
            return false;
        }

        return true;
    }
}
