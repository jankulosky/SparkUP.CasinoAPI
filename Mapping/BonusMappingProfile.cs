using AutoMapper;
using SparkUP.CasinoAPI.DTOs;
using SparkUP.CasinoAPI.Entities;
using SparkUP.CasinoAPI.Models;

namespace SparkUP.CasinoAPI.Mapping
{
    public class BonusMappingProfile : Profile
    {
        public BonusMappingProfile()
        {
            CreateMap<PlayerBonus, PlayerBonusDto>();
            CreateMap<CreateBonusDto, PlayerBonus>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.BonusType, opt => opt.MapFrom(s => s.BonusType.ToString()))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}
