using AutoMapper;
using DataLayer.Domain;
using Dental_Clinic_NET.API.Models.Users.UserLock;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dental_Clinic_NET.API.AutoMapperProfiles
{
    
    public class UserLockProfileMapper : Profile
    {
        public UserLockProfileMapper()
        {
            CreateMap<CreateUserLock, UserLock>();
        }
    }
}
