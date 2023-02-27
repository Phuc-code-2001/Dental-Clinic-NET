using AutoMapper;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTOs;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Dental_Clinic_NET.API.AutoMapperProfiles
{
    public class EnumTypeAutoMapperProfile : Profile
    {
        public EnumTypeAutoMapperProfile()
        {
            CreateMap<TimeManager.Slot, EnumTypeDTO>()
                .ForMember(des => des.Id, act => act.MapFrom(src => src))
                .ForMember(des => des.Name, act => act.MapFrom(src => src.ToString()));

            CreateMap<Room.RoomTypes, EnumTypeDTO>()
                .ForMember(des => des.Id, act => act.MapFrom(src => src))
                .ForMember(des => des.Name, act => act.MapFrom(src => src.ToString()));

            CreateMap<UserType, EnumTypeDTO>()
                .ForMember(des => des.Id, act => act.MapFrom(src => src))
                .ForMember(des => des.Name, act => act.MapFrom(src => src.ToString()));

        }
    }
}
