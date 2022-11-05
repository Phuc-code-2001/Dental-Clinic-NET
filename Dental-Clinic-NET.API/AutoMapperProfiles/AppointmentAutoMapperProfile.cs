using AutoMapper;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Models.Appointments;
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

            CreateMap<Appointment.States, EnumTypeDTO>()
                .ForMember(des => des.Id, opt => opt.MapFrom(src => src))
                .ForMember(des => des.Name, opt => opt.MapFrom(src => src.ToString()));

            CreateMap<CreateAppointment, Appointment>()
                .ForMember(des => des.Date, opt => opt.MapFrom(src => src.Date.Value))
                .ForMember(des => des.Slot, opt => opt.MapFrom(src => src.Slot.Value));

        }

        public static string ConvertSlotToStrTime(Slot slot)
        {
            if(TimeManager.Instance.ContainsKey(slot))
            {
                TimeSpan timer = TimeManager.Instance[slot].Value;
                return $"{timer.Hours}:{timer.Minutes}";
            }

            return "No defined";
        }
    }
}
