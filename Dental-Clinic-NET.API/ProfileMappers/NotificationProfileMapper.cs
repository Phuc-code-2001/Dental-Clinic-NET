using AutoMapper;
using DataLayer.Domain;
using DataLayer.Extensions;
using Dental_Clinic_NET.API.DTOs;
using Dental_Clinic_NET.API.Utils;

namespace Dental_Clinic_NET.API.ProfileMappers
{
    public class NotificationProfileMapper : Profile
    {
        public NotificationProfileMapper()
        {
            CreateMap<Notification, NotificationDTO>()
                .ForMember(des => des.CreatedFormated, opt => opt.MapFrom(src => TimeManager.TranslateTimeToAgo(src.TimeCreated.Value)))
                .ForMember(des => des.Category, opt => opt.MapFrom(src => src.Category.ToString()));
        }

    }
}
