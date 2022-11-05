using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Domain
{
    public class AppointmentDocument : BaseEntity
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Appointment))]
        public int AppointmentId { get; set; }

        public string Title { get; set; }
        public MediaFile Document { get; set; }

        public DocumentTags Tag { get; set; }

        public enum DocumentTags
        {
            Patient,
            Doctor,
        }

    }



}
