using Application.Common.Resources;
using Application.Services.Models.Users;
using FluentValidation;

namespace Application.Services.Validators.Users;

public class ResetPasswordRequestValidator : AbstractValidator<UserRequestModels.ResetPassword>
{
    public ResetPasswordRequestValidator()
    {
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
            .Must(EqualsPasswords).When(x => x.Password is not null || x.ConfirmPassword is not null)
            .WithMessage("Password not match confirm password field");
    }

    private bool EqualsPasswords(UserRequestModels.ResetPassword requestModel)
    {
        if (!requestModel.Password.Equals(requestModel.ConfirmPassword))
        {
            return false;
        }

        return true;
    }
}
