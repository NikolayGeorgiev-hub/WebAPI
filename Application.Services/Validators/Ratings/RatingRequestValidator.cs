using Application.Services.Models.Ratings;
using FluentValidation;

namespace Application.Services.Validators.Ratings;

public class RatingRequestValidator : AbstractValidator<RatingRequestModel>
{
    public RatingRequestValidator()
    {
        RuleFor(x => x.ProductId)
           .NotEmpty()
           .WithMessage("{PropertyName} is required");

        RuleFor(x => x.ProductId)
           .Must(ValidatorHelper.IsValidGuid)
           .WithMessage("Invalid value");

        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage("{PropertyName} is required");

        RuleFor(x => x.Value)
            .ExclusiveBetween(0, 6)
            .WithMessage("Rating value must by between {MinValue} - {MaxValue} stars");
    }
}
