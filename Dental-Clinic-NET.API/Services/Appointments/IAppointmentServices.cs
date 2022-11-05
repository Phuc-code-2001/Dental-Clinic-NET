using DataLayer.Domain;
using System;
using static DataLayer.Domain.TimeManager;

namespace Dental_Clinic_NET.API.Services.Appointments
{
    public interface IAppointmentServices
    {
        public Doctor FindDoctorForAppointment(Appointment appointment);
        public bool IsFreeTime(DateTime Date, Slot slot);
        public Room FindRoomForAppointment(Appointment appointment);

    }
}
