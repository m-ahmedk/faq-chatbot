using FAQDemo.API.DTOs.Faq;
using FluentValidation;

namespace FAQDemo.API.Validators.Faq
{
    public class UpdateFaqDtoValidator : AbstractValidator<UpdateFaqDto>
    {
        public UpdateFaqDtoValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);

            When(x => x.Question != null, () =>
            {
                RuleFor(x => x.Question).MinimumLength(5);
            });

            When(x => x.Answer != null, () =>
            {
                RuleFor(x => x.Answer).MinimumLength(5);
            });
        }
    }
}
