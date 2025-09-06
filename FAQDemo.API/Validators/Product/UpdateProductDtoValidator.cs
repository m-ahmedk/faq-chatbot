// Validators/Product/UpdateProductDtoValidator.cs
using FluentValidation;
using FAQDemo.API.DTOs.Product;

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

        When(x => x.Name != null, () =>
        {
            RuleFor(x => x.Name).NotEmpty().MinimumLength(2);
        });

        When(x => x.Price.HasValue, () =>
        {
            RuleFor(x => x.Price).GreaterThan(0);
        });

        When(x => x.Quantity.HasValue, () =>
        {
            RuleFor(x => x.Quantity).GreaterThan(0);
        });
    }
}