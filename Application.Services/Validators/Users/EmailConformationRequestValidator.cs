using Application.Common.Resources;
using Application.Services.Models.Users;
using FluentValidation;

namespace Application.Services.Validators.Users;

public class EmailConformationRequestValidator : AbstractValidator<UserRequestModels.IdentityToken>
{
    public EmailConformationRequestValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Invalid token value");


        RuleFor(x => x.Email)
             .NotEmpty()
            .WithMessage("{PropertyName} is required");

        RuleFor(x => x.Email)
            .Must(ValidatorHelper.ValidateEmailAddress)
            .When(x => x.Email is not null)
            .WithMessage(Messages.InvalidEmailAddressFormat);

        RuleFor(x => x.Email)
            .Length(5, 100)
            .WithMessage("{PropertyName} must by between {MinLength} - {MaxLength} symbols");
    }
}
