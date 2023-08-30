using Application.Services.Models.Roles;
using FluentValidation;

namespace Application.Services.Validators.Administration;

public class EditRoleValidator : AbstractValidator<RoleRequestModels.Edit>
{
    public EditRoleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("{PropertyName} is required");

        RuleFor(x => x.Name)
            .Length(3, 50)
            .WithMessage("The {PropertyName} must by between {MinLength} - {MaxLength} symbols");
    }
}
