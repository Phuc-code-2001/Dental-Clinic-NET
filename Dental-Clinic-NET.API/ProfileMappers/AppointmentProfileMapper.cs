using AutoMapper;
using DataLayer.Domain;
using DataLayer.Extensions;
using Dental_Clinic_NET.API.DTOs;
using Dental_Clinic_NET.API.Models.Appointments;
using System;

namespace Dental_Clinic_NET.API.ProfileMappers
{
    public class AppointmentProfileMapper : Profile
    {
        public AppointmentProfileMapper()
        {
            CreateMap<Appointment, AppointmentDTO>()
                .ForMember(des => des.Time, opt => opt.MapFrom(src => TimeManager.Instance.TryConvertToStrTime(src.Slot)))
                .ForMember(des => des.State, opt => opt.MapFrom(src => src.State.ToString()))
                .ForMember(des => des.From, opt => opt.MapFrom(src => src.Date + TimeManager.Instance.GetTime(src.Slot).Value));

            CreateMap<Appointment.States, EnumTypeDTO>()
                .ForMember(des => des.Id, opt => opt.MapFrom(src => src))
                .ForMember(des => des.Name, opt => opt.MapFrom(src => src.ToString()));

            CreateMap<CreateAppointment, Appointment>()
                .ForMember(des => des.Date, opt => opt.MapFrom(src => src.Date.Value))
                .ForMember(des => des.Slot, opt => opt.MapFrom(src => src.Slot));

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
                .ForMember(des => des.State, opt => opt.MapFrom(src => src.State.ToString()))
                .ForMember(des => des.From, opt => opt.MapFrom(src => src.Date + TimeManager.Instance.GetTime(src.Slot).Value));

        }

    }
}
