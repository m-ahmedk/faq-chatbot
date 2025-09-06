using AutoMapper;
using FAQDemo.API.DTOs.Product;
using FAQDemo.API.Models;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        // Entity -> DTO
        CreateMap<Product, ProductDto>();

        // Create DTO -> Entity (ignore Id, DB generates it)
        CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        // Update DTO -> Entity (only map non-null values)
        CreateMap<UpdateProductDto, Product>()
        .ForMember(dest => dest.Price, opt =>
            opt.Condition(src => src.Price.HasValue))
        .ForMember(dest => dest.Quantity, opt =>
            opt.Condition(src => src.Quantity.HasValue))
        .ForMember(dest => dest.Name, opt =>
            opt.Condition(src => !string.IsNullOrWhiteSpace(src.Name)));

    }
}
