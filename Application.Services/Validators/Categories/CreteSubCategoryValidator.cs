using Application.Services.Models.Categories;
using FluentValidation;

namespace Application.Services.Validators.Categories;

public class CreteSubCategoryValidator : AbstractValidator<CreateSubCategoryRequestModel>
{
    public CreteSubCategoryValidator()
    {
        RuleFor(x => x.Name)
           .NotEmpty()
           .WithMessage("{PropertyName} is required");

        RuleFor(x => x.Name)
            .Length(3, 50)
            .WithMessage("{PropertyName} must by between {MinLength} - {MaxLength} symbols");

        RuleFor(x => x.CategoryId)
            .Must(ValidatorHelper.IsValidGuid)
            .WithMessage("Invalid value");
    }
}
