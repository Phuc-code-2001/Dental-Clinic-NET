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
        public string PatientId { get; set; }

        public string DoctorId { get; set; }

        public TimeManager.SlotManager? Slot { get; set; }
        
        [DataType(DataType.Date, ErrorMessage = "Format 'MM/dd/yyyy' required!")]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Format 'MM/dd/yyyy' required!")]
        public DateTime? EndDate { get; set; }

        public int? ServiceId { get; set; }

        public Appointment.States? State { get; set; }

        public string PhoneNumber { get; set; }

        public IQueryable<Appointment> Filter(IQueryable<Appointment> queries)
        {

            if(PatientId != null)
            {
                queries = queries.Where(apt => apt.PatientId == PatientId);
            }
            
            if(DoctorId != null)
            {
                queries = queries.Where(apt => apt.DoctorId == DoctorId);
            }

            if (Slot != null)
            {
                queries = queries.Where(apt => apt.Slot == Slot.Value);
            }
            if (State != null)
            {
                queries = queries.Where(apt => apt.State == State.Value);
            }

            if (ServiceId != null)
            {
                queries = queries.Where(apt => apt.ServiceId == ServiceId);
            }

            if (StartDate != null)
            {
                queries = queries.Where(apt => apt.Date >= StartDate.Value);
            }

            if (EndDate != null)
            {
                queries = queries.Where(apt => apt.Date <= EndDate.Value);
            }

            if(!string.IsNullOrWhiteSpace(PhoneNumber))
            {
                queries = queries.Where(apt => apt.Patient.BaseUser.PhoneNumber == PhoneNumber);
            }

            return queries;
        }

    }
}
