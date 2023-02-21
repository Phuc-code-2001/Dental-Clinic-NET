using AutoMapper;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Models.Doctors;
using Dental_Clinic_NET.API.Models.Users;
using System;

namespace Dental_Clinic_NET.API.AutoMapperProfiles
{
    public class DoctorProfileMapper : Profile
    {
        public DoctorProfileMapper()
        {

            CreateMap<UpdateDoctor, Doctor>()
                .ForMember(des => des.Verified, opt => opt.MapFrom(src => src.Verified.Value))
                .ForAllMembers(opt =>
                {
                    opt.Condition((src, des, member) =>
                    {
                        return member != null;
                    });
                });

            CreateMap<Doctor, DoctorDTO>();

            // Lite
            CreateMap<Doctor, DoctorDTOLite>();
        }
    }
}
