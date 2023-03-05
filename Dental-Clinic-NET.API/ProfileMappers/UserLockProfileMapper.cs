using AutoMapper;
using DataLayer.Domain;
using Dental_Clinic_NET.API.Models.Users.UserLock;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dental_Clinic_NET.API.ProfileMappers
{

    public class UserLockProfileMapper : Profile
    {
        public UserLockProfileMapper()
        {
            CreateMap<CreateUserLock, UserLock>()
                .ForMember(des => des.BaseUserId, opt => opt.MapFrom(src => src.UserId));
        }
    }
}
