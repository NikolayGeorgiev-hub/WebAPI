using Application.Services.Models.Discounts;
using FluentValidation;

namespace Application.Services.Validators.Discounts;

public class CreateDiscountRequestValidator : AbstractValidator<CreteDiscountRequestModel>
{
    public CreateDiscountRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("{PropertyName} is required");

        RuleFor(x => x.Description)
            .Length(5, 500)
            .WithMessage("{PropertyName} must by between {MinLength} - {MaxLength} symbols");

        RuleFor(x => x.Percentage)
            .InclusiveBetween(1, 50)
            .WithMessage("{PropertyName} must by between {From} - {To} %");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .When(x => x.CategoryId is not null)
            .WithMessage("{PropertyName} is required");

        RuleFor(x => x.SubCategoryId)
            .NotEmpty()
            .When(x => x.SubCategoryId is not null)
            .WithMessage("{PropertyName} is required");

        RuleFor(x => x)
            .Custom((value, context) =>
            {
                if (value.CategoryId is null && value.SubCategoryId is null && value.Products is null)
                {
                    context.AddFailure("Select discount target");
                }
            });

        RuleFor(x => x.StartDate)
            .Must(ValidatorHelper.IsValidDate)
            .When(x => x.StartDate is not null)
            .WithMessage("Invalid {PropertyName}");

        RuleFor(x => x.StartDate)
            .Must(startDate => startDate > DateTime.UtcNow)
            .When(x => x.StartDate is not null)
            .WithMessage("{PropertyName} must by after today");

        RuleFor(x => x.EndDate)
            .Must(ValidatorHelper.IsValidDate)
            .When(x => x.EndDate is not null)
            .WithMessage("Invalid {PropertyName}");

        RuleFor(x => x)
            .Custom((value, context) =>
             {
                 if (value.EndDate is null)
                 {
                     context.AddFailure("Select end date");
                 }

             }).When(x => x.StartDate is not null);

        RuleFor(x => x)
            .Custom((value, context) =>
            {
                if (value.StartDate is null)
                {
                    context.AddFailure("Select start date");
                }

            }).When(x => x.EndDate is not null);

        RuleFor(x => x)
            .Custom((value, context) =>
            {
                if (value.EndDate!.Value < value.StartDate!.Value)
                {
                    context.AddFailure("End date must after start date");
                }

            }).When(x => x.EndDate is not null && x.StartDate is not null);
    }
}
