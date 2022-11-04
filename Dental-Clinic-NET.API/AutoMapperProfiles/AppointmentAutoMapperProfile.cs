using AutoMapper;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTO;
using System;
using static DataLayer.Domain.TimeManager;

namespace Dental_Clinic_NET.API.AutoMapperProfiles
{
    public class AppointmentAutoMapperProfile : Profile
    {
        public AppointmentAutoMapperProfile()
        {
            CreateMap<Appointment, AppointmentDTO>()
                .ForMember(des => des.Time, opt => opt.MapFrom(src => ConvertSlotToStrTime(src.Slot)))
                .ForMember(des => des.State, opt => opt.MapFrom(src => src.State.ToString()));
        }

        private string ConvertSlotToStrTime(Slot slot)
        {
            TimeSpan timer = TimeManager.Instance[slot];
            return $"{timer.Hours}:{timer.Minutes}";
        }
    }
}
