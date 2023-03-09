using DataLayer.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using static DataLayer.Extensions.TimeManager;

namespace Dental_Clinic_NET.API.Models.Schedules
{
    public class TimeIdentifier
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }
        public SlotManager Slot { get; set; }

        public override string ToString()
        {
            string dateFormated = Date.Value.ToShortDateString();
            string timeFormated = TimeManager.Instance.TryConvertToStrTime(Slot);

            return $"{dateFormated} {timeFormated}";
        }
    }
}
