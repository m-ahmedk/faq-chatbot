using FAQDemo.API.DTOs.Faq;
using FluentValidation;

namespace FAQDemo.API.Validators.Faq
{
    public class CreateFaqDtoValidator : AbstractValidator<CreateFaqDto>
    {
        public CreateFaqDtoValidator()
        {
            RuleFor(x => x.Question)
                .NotEmpty().WithMessage("Question is required")
                .MinimumLength(5).WithMessage("Question must be at least 5 characters");

            RuleFor(x => x.Answer)
                .NotEmpty().WithMessage("Answer is required")
                .MinimumLength(5).WithMessage("Answer must be at least 5 characters");
        }
    }
}
