using AutoMapper;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTOs;
using Dental_Clinic_NET.API.Models.Doctors;
using Dental_Clinic_NET.API.Models.Users;
using System;

namespace Dental_Clinic_NET.API.ProfileMappers
{
    public class DoctorProfileMapper : Profile
    {
        public DoctorProfileMapper()
        {

            CreateMap<UpdateDoctor, Doctor>()
                .AfterMap((src, dest) =>
                {
                    if (!string.IsNullOrWhiteSpace(src.FullName)) dest.BaseUser.FullName = src.FullName;
                    if (!string.IsNullOrWhiteSpace(src.Address)) dest.BaseUser.Address = src.Address;
                    if (!string.IsNullOrWhiteSpace(src.Gender)) dest.BaseUser.Gender = src.Gender;
                    if (!string.IsNullOrWhiteSpace(src.Major)) dest.Major = src.Major;
                    if (!string.IsNullOrWhiteSpace(src.Phone)) dest.BaseUser.PhoneNumber = src.Phone;
                    if (!string.IsNullOrWhiteSpace(src.Email)) dest.BaseUser.Email = src.Email;
                    if (src.BirthDate != null) dest.BaseUser.BirthDate = src.BirthDate.Value;
                    if (src.Verified != null) dest.Verified = src.Verified.Value;
                });

            CreateMap<Doctor, DoctorDTO>();
            CreateMap<Doctor, DoctorDTOLite>();
        }
    }
}
