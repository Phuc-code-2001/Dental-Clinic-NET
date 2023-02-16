using AutoMapper;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTO;
using FileProcessorServices;
using Microsoft.Extensions.Configuration;

namespace Dental_Clinic_NET.API.AutoMapperProfiles
{
    public class MediaAutoMapperProfile : Profile
    {

        DropboxServices dropboxServices = new DropboxServices();

        public MediaAutoMapperProfile()
        {
            

            CreateMap<FileMedia, MediaFileDTO>()
                .ForMember(des => des.Category, act => act.MapFrom(src => src.Category.ToString()))
                .ForMember(des => des.FileURL, act => act.MapFrom(src => GetLink(src.FilePath)));
        }

        private string GetLink(string path)
        {
            if(path == null) return null;
            return dropboxServices.GetShareLinkAsync(path).Result;
        }
    }
}
