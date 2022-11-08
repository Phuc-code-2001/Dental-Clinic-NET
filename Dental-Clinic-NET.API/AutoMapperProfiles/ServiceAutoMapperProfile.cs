using AutoMapper;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Models.Devices;
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
            CreateMap<Service, ServiceDTO>();

            CreateMap<UpdateService, Service>()
                    .ForAllMembers(opt => opt.Condition((src, des, field) =>
                    {
                        bool condition_01 = field is string && !string.IsNullOrWhiteSpace(field.ToString());
                        bool condition_02 = field is int && int.Parse(field.ToString()) > 0;
                        return condition_01 || condition_02;
                    }));

            // EnumType to handle selectbox

            CreateMap<Service, EnumTypeDTO>()
                .ForMember(des => des.Name, opt => opt.MapFrom(src => src.ServiceCode));

            // Lite
            CreateMap<Service, ServiceDTOLite>();

        }
    }
}
