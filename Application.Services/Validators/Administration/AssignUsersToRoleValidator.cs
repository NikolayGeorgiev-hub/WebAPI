using Application.Services.Models.Roles;
using FluentValidation;

namespace Application.Services.Validators.Administration;

public class AssignUsersToRoleValidator : AbstractValidator<RoleRequestModels.AssignUsers>
{
    public AssignUsersToRoleValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage("{PropertyName} is required");

        RuleFor(x => x.RoleId)
            .Must(ValidatorHelper.IsValidGuid)
            .WithMessage("Invalid role id");

        RuleFor(x => x.UsersId.Count)
            .ExclusiveBetween(0, 10)
            .When(x => x.UsersId is not null)
            .WithMessage("Must select 1 to 10 users");

        RuleForEach(x => x.UsersId)
            .NotEmpty()
            .When(x => x.UsersId is not null)
            .WithMessage("Invalid user id");

        RuleForEach(x => x.UsersId)
            .Must(ValidatorHelper.IsValidGuid)
            .When(x => x.UsersId is not null)
            .WithMessage("Invalid user id");

    }
}
