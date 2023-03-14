using AutoMapper;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTOs;
using Dental_Clinic_NET.API.Models.Services;
using System;

namespace Dental_Clinic_NET.API.ProfileMappers
{
    public class ServiceProfileMapper : Profile
    {
        public ServiceProfileMapper()
        {
            CreateMap<CreateService, Service>();
            CreateMap<Service, ServiceDTO>();

            CreateMap<UpdateService, Service>()
                    .ForAllMembers(opt => opt.Condition((UpdateService src, Service des, object field) =>
                    {
                        return field != null;
                    }));


            // EnumType to handle selectbox

            CreateMap<Service, EnumTypeDTO>()
                .ForMember(des => des.Name, opt => opt.MapFrom(src => src.ServiceCode));

            // Lite
            CreateMap<Service, ServiceDTOLite>();

        }
    }
}
