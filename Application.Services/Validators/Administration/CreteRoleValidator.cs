using Application.Services.Models.Roles;
using FluentValidation;

namespace Application.Services.Validators.Administration;

public class CreteRoleValidator : AbstractValidator<RoleRequestModels.Crete>
{
    public CreteRoleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("{PropertyName} is required");

        RuleFor(x => x.Name)
            .Length(3, 50)
            .WithMessage("The {PropertyName} must by between {MinLength} - {MaxLength} symbols");
    }
}
