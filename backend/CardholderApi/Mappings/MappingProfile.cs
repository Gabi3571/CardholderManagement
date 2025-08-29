using AutoMapper;
using CardholderApi.DTOs;
using CardholderApi.Entities;

namespace CardholderApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Cardholder, CardholderDTO>().ReverseMap();

            CreateMap<CreateCardholderDTO, Cardholder>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()));

            CreateMap<UpdateCardholderDTO, Cardholder>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
