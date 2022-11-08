using AutoMapper;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTO;
using FileProcessorServices;

namespace Dental_Clinic_NET.API.AutoMapperProfiles
{
    public class MediaAutoMapperProfile : Profile
    {
        public MediaAutoMapperProfile() 
        {
            CreateMap<MediaFile, MediaFileDTO>()
                .ForMember(des => des.Category, act => act.MapFrom(src => src.Category.ToString()))
                .ForMember(des => des.FileURL, act => act.MapFrom(src => GetLink(src.FilePath)));
        }

        private string GetLink(string path)
        {
            if(path == null) return null;
            DropboxServices dropboxServices = new DropboxServices();
            return dropboxServices.GetShareLinkAsync(path).Result;
        }
    }
}
