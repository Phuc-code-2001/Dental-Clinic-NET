using AutoMapper;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Models.Contacts;
using Dental_Clinic_NET.API.Models.Room;
using System.Linq;

namespace Dental_Clinic_NET.API.AutoMapperProfiles
{
    public class RoomAutoMapperProfile : Profile
    {
        public RoomAutoMapperProfile()
        {
            CreateMap<CreateRoom, Room>();
            CreateMap<Room, RoomDTO>()
                .ForMember(des => des.DeviceNames, act => act.MapFrom(src => src.Devices.Select(d => d.DeviceName).ToList()))
                .ForMember(des => des.RoomType, act => act.MapFrom(src => src.RoomType.ToString()));
        }
    }
}
