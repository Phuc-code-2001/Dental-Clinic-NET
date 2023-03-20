using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Domain
{
    public class SegmentationResult
    {
        [Key]
        public int Id { get; set; }
        public Appointment Appointment { get; set; }
        public BaseUser Technican { get; set; }

        public string InputImageURL { get; set; }
        public string ModelName { get; set; }
        public int TeethCount { get; set; }

        public List<SegmentationImageResult> ImageResultSet { get; set; }

        public class SegmentationImageResult
        {
            [Key]
            public int Id { get; set; }
            public string Title { get; set; }
            public string ImageURL { get; set; }
        }
    }

}
