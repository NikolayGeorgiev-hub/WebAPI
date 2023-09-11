using Application.Services.Models.Products;
using FluentValidation;

namespace Application.Services.Validators.Products;

public class EditProductValidator : AbstractValidator<EditProductRequestModel>
{
    public EditProductValidator()
    {
        RuleFor(x => x.Name)
           .NotEmpty()
           .WithMessage("{PropertyName} is required");

        RuleFor(x => x.Name)
            .Length(3, 50)
            .WithMessage("{PropertyName} must by between {MinLength} - {MaxLength} symbols");

        RuleFor(x => x.Description)
           .NotEmpty()
           .WithMessage("{PropertyName} is required");

        RuleFor(x => x.Description)
            .Length(10, 1000)
            .WithMessage("{PropertyName} must by between {MinLength} - {MaxLength} symbols");

        RuleFor(x => x.Price)
            .ExclusiveBetween(0.1m, 50000m)
            .WithMessage("{PropertyName} must by in range {MinValue} - {MaxValue}");

        RuleFor(x => x.Quantity)
           .ExclusiveBetween(-1, 50000)
           .WithMessage("{PropertyName} must by in range {MinValue} - {MaxValue} items");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("{PropertyName} is required");

        RuleFor(x => x.CategoryId)
           .Must(ValidatorHelper.IsValidGuid)
           .WithMessage("Invalid value");

        RuleFor(x => x.SubCategoryId)
            .NotEmpty()
            .WithMessage("{PropertyName} is required");

        RuleFor(x => x.SubCategoryId)
           .Must(ValidatorHelper.IsValidGuid)
           .WithMessage("Invalid value");
    }
}
