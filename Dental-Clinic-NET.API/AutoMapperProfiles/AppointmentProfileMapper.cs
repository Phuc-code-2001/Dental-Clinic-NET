using AutoMapper;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTOs;
using Dental_Clinic_NET.API.Models.Appointments;
using System;
using static DataLayer.Domain.TimeManager;

namespace Dental_Clinic_NET.API.AutoMapperProfiles
{
    public class AppointmentProfileMapper : Profile
    {
        public AppointmentProfileMapper()
        {
            CreateMap<Appointment, AppointmentDTO>()
                .ForMember(des => des.Time, opt => opt.MapFrom(src => TimeManager.Instance.TryConvertToStrTime(src.Slot)))
                .ForMember(des => des.State, opt => opt.MapFrom(src => src.State.ToString()));

            CreateMap<Appointment.States, EnumTypeDTO>()
                .ForMember(des => des.Id, opt => opt.MapFrom(src => src))
                .ForMember(des => des.Name, opt => opt.MapFrom(src => src.ToString()));

            CreateMap<CreateAppointment, Appointment>()
                .ForMember(des => des.Date, opt => opt.MapFrom(src => src.Date.Value))
                .ForMember(des => des.Slot, opt => opt.MapFrom(src => src.Slot.Value));
            
            CreateMap<UpdateAppointment, Appointment>()
                .ForMember(des => des.Date, opt => opt.MapFrom(src => src.Date.Value))
                .ForMember(des => des.Slot, opt => opt.MapFrom(src => src.Slot.Value))
                .ForMember(des => des.ServiceId, opt => opt.MapFrom(src => src.ServiceId.Value))
                .ForAllMembers(opt => opt.Condition((des, src, field) => field != null));

            // Appointment document
            CreateMap<AddDocumentModel, Document>();

            CreateMap<Document, DocumentDTO>()
                .ForMember(des => des.Tag, opt => opt.MapFrom(src => src.Tag.ToString()));

            // Appointment Lite
            CreateMap<Appointment, AppointmentDTOLite>()
                .ForMember(des => des.Time, opt => opt.MapFrom(src => TimeManager.Instance.TryConvertToStrTime(src.Slot)))
                .ForMember(des => des.State, opt => opt.MapFrom(src => src.State.ToString()));

        }

    }
}
