// Validators/Product/CreateProductDtoValidator.cs
using FluentValidation;
using FAQDemo.API.DTOs.Product;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(2);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}