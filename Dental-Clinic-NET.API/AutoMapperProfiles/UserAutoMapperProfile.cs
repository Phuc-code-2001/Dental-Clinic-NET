using AutoMapper;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Models.Users;
using System.Collections.Generic;
using System.Linq;

namespace Dental_Clinic_NET.API.AutoMapperProfiles
{
    public class UserAutoMapperProfile : Profile
    {
        public UserAutoMapperProfile()
        {
            CreateMap<BasicRegisterModel, BaseUser>();


            CreateMap<BaseUser, UserDTO>()
                .ForMember(des => des.Role, act => act.MapFrom(src => src.Type.ToString()));

        }

    }
}
