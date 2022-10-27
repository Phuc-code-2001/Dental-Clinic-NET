using AutoMapper;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Models.Room;
using Dental_Clinic_NET.API.Models.Services;
using System.Linq;

namespace Dental_Clinic_NET.API.AutoMapperProfiles
{
    public class ServiceAutoMapperProfile : Profile
    {
        public ServiceAutoMapperProfile()
        {
            CreateMap<CreateService, Service>();
            CreateMap<Service, ServiceDTO>()
                .ForMember(des => des.DeviceNames, act => act.MapFrom(src => src.Devices.Select(d => d.DeviceName).ToList()))
                .ForMember(des => des.DeviceIdList, act => act.MapFrom(src => src.Devices.Select(d => d.Id).ToList()));
        }
    }
}
