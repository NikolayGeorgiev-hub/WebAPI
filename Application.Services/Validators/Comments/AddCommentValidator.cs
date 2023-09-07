using Application.Services.Models.Comments;
using FluentValidation;

namespace Application.Services.Validators.Comments;

public class AddCommentValidator : AbstractValidator<AddCommentRequestModel>
{
    public AddCommentValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("{PropertyName} is required");

        RuleFor(x => x.Content)
            .Length(3, 500)
            .WithMessage("The {PropertyName} must by between {MinLength} - {MaxLength} symbols");
    }
}
