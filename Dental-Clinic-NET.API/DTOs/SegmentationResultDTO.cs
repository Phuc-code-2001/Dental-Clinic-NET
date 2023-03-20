using DataLayer.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.DTOs
{
    public class SegmentationResultDTO
    {
        public int Id { get; set; }
        public AppointmentDTOLite Appointment { get; set; }
        public UserDTOLite Technican { get; set; }

        public string InputImageURL { get; set; }
        public string ModelName { get; set; }
        public int TeethCount { get; set; }

        public List<SegmentationImageResultDTO> ImageResultSet { get; set; }

        public class SegmentationImageResultDTO
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string ImageURL { get; set; }
        }
    }
}
