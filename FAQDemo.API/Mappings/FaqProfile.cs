using AutoMapper;
using FAQDemo.API.Models;
using FAQDemo.API.DTOs.Faq;

namespace FAQDemo.API.Mappings
{
    public class FaqProfile : Profile
    {
        public FaqProfile()
        {
            // Entity -> DTO (response)
            CreateMap<Faq, FaqDto>();

            // Create DTO -> Entity
            CreateMap<CreateFaqDto, Faq>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // DB generates
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // handled by BaseEntity
                .ForMember(dest => dest.LastModifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Vector, opt => opt.Ignore()); // ignore vector, handled by embedding service

            // Update DTO -> Entity (only map non-null values)
            CreateMap<UpdateFaqDto, Faq>()
                .ForMember(dest => dest.Question, opt =>
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.Question)))
                .ForMember(dest => dest.Answer, opt =>
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.Answer)))
                .ForMember(dest => dest.Vector, opt => opt.Ignore()) // ignore vector, handled by embedding service
            ;
        }
    }
}
