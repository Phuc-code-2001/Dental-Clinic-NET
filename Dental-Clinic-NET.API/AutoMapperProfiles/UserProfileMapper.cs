﻿using AutoMapper;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dental_Clinic_NET.API.AutoMapperProfiles
{
    public class UserProfileMapper : Profile
    {
        public UserProfileMapper()
        {
            CreateMap<BasicRegisterModel, BaseUser>();

            CreateMap<BaseUser, UserDTO>()
                .ForMember(des => des.IsLock, act => act.MapFrom(src => src.UserLock.IsLockCalculated))
                .ForMember(des => des.Role, act => act.MapFrom(src => src.Type.ToString()));

            CreateMap<UpdateUserModel, BaseUser>()
                .ForAllMembers(opt => opt.Condition((src, des, field) =>
                {
                    bool condition_01 = field is string && !string.IsNullOrWhiteSpace(field.ToString());
                    bool condition_02 = field is DateTime? && field != null;
                    return condition_01 || condition_02;
                }));

            // Patient
            CreateMap<Patient, PatientDTO>();
            CreateMap<Patient, PatientDTOLite>();
            CreateMap<BaseUser, UserDTOLite>();

        }

    }
}