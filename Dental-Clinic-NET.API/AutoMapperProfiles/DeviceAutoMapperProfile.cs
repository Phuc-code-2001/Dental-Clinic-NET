using AutoMapper;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Models.Devices;
using Dental_Clinic_NET.API.Models.Room;
using System.Linq;

namespace Dental_Clinic_NET.API.AutoMapperProfiles
{
    public class DeviceAutoMapperProfile : Profile
    {
        public DeviceAutoMapperProfile()
        {
            CreateMap<CreateDevice, Device>();
            CreateMap<Device, DeviceDTO>().ForMember(des => des.ServiceNames, act => act.MapFrom(src => src.Services.Select(d => d.ServiceCode).ToList()));
        }
    }
}
