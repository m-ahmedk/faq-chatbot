using AutoMapper;
using FAQDemo.API.Models;
using FAQDemo.API.DTOs.Order;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        // Entity -> DTO
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty));

        // Create DTO -> Entity
        CreateMap<CreateOrderDto, Order>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())       // DB generates
            .ForMember(dest => dest.UserId, opt => opt.Ignore())   // set in service from claims
            .ForMember(dest => dest.User, opt => opt.Ignore())     // handled by EF
            .ForMember(dest => dest.Status, opt => opt.Ignore())   // default set in service
            .ForMember(dest => dest.Items, opt => opt.Ignore());   // handled manually in service

        CreateMap<CreateOrderItemDto, OrderItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())       // DB generates
            .ForMember(dest => dest.OrderId, opt => opt.Ignore())  // set in service
            .ForMember(dest => dest.Order, opt => opt.Ignore())    // handled by EF
            .ForMember(dest => dest.UnitPrice, opt => opt.Ignore()) // price snapshot set in service
            .ForMember(dest => dest.Product, opt => opt.Ignore());  // service fetches Product
    }
}
