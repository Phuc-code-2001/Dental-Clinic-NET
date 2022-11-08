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

            CreateMap<Device, RoomDTO.DeviceInnerDTO>();
            CreateMap<Room, RoomDTO>();

            CreateMap<UpdateRoom, Room>()
                .ForAllMembers(opt => opt.Condition((src, des, field) =>
                {
                    bool condition_01 = field != null && field is not string;
                    bool condition_02 = field is string && !string.IsNullOrWhiteSpace(field.ToString());

                    return condition_01 || condition_02;
                }));

            // Lite
            CreateMap<Room, RoomDTOLite>();
        }
    }
}
