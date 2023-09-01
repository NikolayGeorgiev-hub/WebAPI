using Application.Services.Models.Categories;
using FluentValidation;

namespace Application.Services.Validators.Categories;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryRequestModel>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("{PropertyName} is required");

        RuleFor(x => x.Name)
            .Length(3, 50)
            .WithMessage("{PropertyName} must by between {MinLength} - {MaxLength} symbols");
    }
}
