using AutoMapper;
using DataLayer.Schemas;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Models.Users;

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
