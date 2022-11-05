using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DataLayer.Domain;
using static DataLayer.Domain.Appointment;

namespace Dental_Clinic_NET.API.Models.Appointments
{
    public class AppointmentFilter
    {
        public string PatientId { get; set; }

        public string DoctorId { get; set; }

        public Nullable<Int32> Slot { get; set; }
        
        [DataType(DataType.Date, ErrorMessage = "Format 'MM/dd/yyyy' required!")]
        public Nullable<DateTime> StartDate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Format 'MM/dd/yyyy' required!")]
        public Nullable<DateTime> EndDate { get; set; }

        public Nullable<Int32> ServiceId { get; set; }

        public Nullable<Int32> State { get; set; }

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
                queries = queries.Where(apt => (int) apt.Slot == Slot.Value);
            }
            if (State != null)
            {
                queries = queries.Where(apt => (int) apt.State == State.Value);
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

            return queries;
        }

    }
}
