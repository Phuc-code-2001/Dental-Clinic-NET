using AutoMapper;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTOs;

namespace Dental_Clinic_NET.API.AutoMapperProfiles
{
    public class NotificationProfileMapper : Profile
    {
        public NotificationProfileMapper()
        {
            CreateMap<Notification, NotificationDTO>()
                .ForMember(des => des.Category, opt => opt.MapFrom(src => src.Category.ToString()));
        }
    }
}
