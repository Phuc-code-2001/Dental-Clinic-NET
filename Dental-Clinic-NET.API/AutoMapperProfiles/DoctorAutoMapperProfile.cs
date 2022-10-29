using AutoMapper;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Models.Doctors;
using Dental_Clinic_NET.API.Models.Users;
using System;

namespace Dental_Clinic_NET.API.AutoMapperProfiles
{
    public class DoctorAutoMapperProfile : Profile
    {
        public DoctorAutoMapperProfile()
        {
            CreateMap<UpdateDoctor, Doctor>()
                .ForMember(des => des.Major, act => act.MapFrom(src => src.Major.ToString()));
            CreateMap<Doctor, DoctorDTO>();
    
        }
    }
}
