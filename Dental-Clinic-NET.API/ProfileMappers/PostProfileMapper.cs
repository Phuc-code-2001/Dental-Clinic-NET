using AutoMapper;
using DataLayer.DataContexts;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTOs;
using Dental_Clinic_NET.API.Models.Posts;
using System.Collections.Generic;
using System.Linq;

namespace Dental_Clinic_NET.API.ProfileMappers
{
    public class PostProfileMapper : Profile
    {

        public PostProfileMapper()
        {

            CreateMap<Post, PostDTO>()
                .ForMember(des => des.Creator, opt => opt.MapFrom(src => src.Creator.UserName));

            CreateMap<CreatePost, Post>();

            CreateMap<UpdatePost, Post>();

        }
    }
}
