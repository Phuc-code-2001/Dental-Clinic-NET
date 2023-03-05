using AutoMapper;
using DataLayer.Domain;
using DataLayer.Extensions;
using Dental_Clinic_NET.API.DTOs;

namespace Dental_Clinic_NET.API.ProfileMappers
{
    public class EnumTypeProfileMapper : Profile
    {
        public EnumTypeProfileMapper()
        {
            CreateMap<TimeManager.SlotManager, EnumTypeDTO>()
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
