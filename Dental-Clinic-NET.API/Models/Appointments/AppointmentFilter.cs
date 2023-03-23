using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DataLayer.Domain;
using DataLayer.Extensions;
using Dental_Clinic_NET.API.Utils;
using static DataLayer.Domain.Appointment;

namespace Dental_Clinic_NET.API.Models.Appointments
{
    public class AppointmentFilter : PageFilter
    {
        public string PatientId { get; set; } = string.Empty; 

        public string DoctorId { get; set; } = string.Empty; 

        public TimeManager.SlotManager? Slot { get; set; }
        
        [DataType(DataType.Date, ErrorMessage = "Format 'MM/dd/yyyy' required!")]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Format 'MM/dd/yyyy' required!")]
        public DateTime? EndDate { get; set; }

        public int? ServiceId { get; set; }

        public Appointment.States? State { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;


        public IQueryable<Appointment> Filter(IQueryable<Appointment> queries)
        {

            queries = queries.Where(apt => (Slot == null || apt.Slot == Slot.Value) &&
                        (State == null || apt.State == State.Value) &&
                        (ServiceId == null || apt.ServiceId == ServiceId.Value) &&
                        (StartDate == null || apt.Date >= StartDate.Value) &&
                        (EndDate == null || apt.Date <= EndDate.Value) &&
                        (string.IsNullOrWhiteSpace(PatientId) || apt.PatientId.Contains(PatientId)) &&
                        (string.IsNullOrWhiteSpace(DoctorId) || apt.DoctorId.Contains(DoctorId)) &&
                        (
                            (string.IsNullOrWhiteSpace(UserName) || apt.Patient.BaseUser.UserName.Contains(UserName)) ||
                            (string.IsNullOrWhiteSpace(PhoneNumber) || apt.Patient.BaseUser.PhoneNumber.Contains(PhoneNumber)))
                        );

            return queries;
        }

    }
}
