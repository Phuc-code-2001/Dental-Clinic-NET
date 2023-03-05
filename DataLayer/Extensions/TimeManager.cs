using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Extensions
{

    public class TimeManager
    {
        private static TimeManager _instance;

        public static TimeManager Instance
        {
            get
            {
                return _instance ??= new TimeManager();
            }
        }

        private Dictionary<SlotManager, TimeSpan?> _manager = new Dictionary<SlotManager, TimeSpan?>();

        public enum SlotManager
        {
            Slot_01,
            Slot_02,
            Slot_03,
            Slot_04,
            Slot_05,
            Slot_06,
            Slot_07,
            Slot_08,
            Slot_09,
        }

        private TimeManager()
        {
            _manager.Add(SlotManager.Slot_01, new TimeSpan(8, 30, 0));
            _manager.Add(SlotManager.Slot_02, new TimeSpan(9, 0, 0));
            _manager.Add(SlotManager.Slot_03, new TimeSpan(9, 30, 0));
            _manager.Add(SlotManager.Slot_04, new TimeSpan(10, 0, 0));
            _manager.Add(SlotManager.Slot_05, new TimeSpan(14, 0, 0));
            _manager.Add(SlotManager.Slot_06, new TimeSpan(14, 30, 0));
            _manager.Add(SlotManager.Slot_07, new TimeSpan(15, 0, 0));
            _manager.Add(SlotManager.Slot_08, new TimeSpan(15, 30, 0));
            _manager.Add(SlotManager.Slot_09, new TimeSpan(16, 0, 0));
        }

        public TimeSpan? GetTime(SlotManager slot)
        {
            return _manager.GetValueOrDefault(slot, null);
        }

        public string ConvertToStrTime(TimeSpan duration)
        {
            return string.Format("{0}h{1}m", duration.Hours.ToString("00"), duration.Minutes.ToString("00"));
        }

        public string TryConvertToStrTime(SlotManager slot)
        {
            var time = GetTime(slot);
            return time.HasValue ? ConvertToStrTime(time.Value) : "ambiguous!";
        }

        public static string TranslateTimeToAgo(DateTime datetime)
        {

            TimeSpan duration = DateTime.Now - datetime;

            int totalSeconds = (int)Math.Round(duration.TotalSeconds, 0);

            int minuteUnit = 60;
            int hourUnit = 60 * minuteUnit;
            int dayUnit = 24 * hourUnit;

            string message = "";
            int dayCount = totalSeconds / dayUnit;
            if (dayCount > 0)
            {
                message += string.Format("{0} day{1} ", dayCount, dayCount > 1 ? "s" : "");
                totalSeconds %= dayUnit;
            }
            else
            {
                int hourCount = totalSeconds / hourUnit;
                if (hourCount > 0)
                {
                    message += string.Format("{0} hour{1} ", hourCount, hourCount > 1 ? "s" : "");
                    totalSeconds %= hourUnit;
                }

                int minuteCount = totalSeconds / minuteUnit;
                if (minuteCount > 0)
                {
                    message += string.Format("{0} minute{1} ", minuteCount, minuteCount > 1 ? "s" : "");
                    totalSeconds %= minuteUnit;
                }

                if (hourCount == 0)
                {
                    if (totalSeconds > 0)
                    {
                        message += string.Format("{0} second{1} ", totalSeconds, totalSeconds > 1 ? "s" : "");
                    }
                }

            }

            message += "ago";

            return message;
        }
    }
}