using AutoMapper;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTO;

namespace Dental_Clinic_NET.API.AutoMapperProfiles
{
    public class MediaAutoMapperProfile : Profile
    {
        public MediaAutoMapperProfile() 
        {
            CreateMap<MediaFile, MediaFileDTO>()
                .ForMember(des => des.Category, act => act.MapFrom(src => src.Category.ToString()));
        }
    }
}
