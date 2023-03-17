using AutoMapper;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dental_Clinic_NET.API.ProfileMappers
{
    public class FeedBackProfileMapper : Profile
    {

        public FeedBackProfileMapper()
        {
            CreateMap<FeedBack, FeedBackDTO>();
        }

    }
}
