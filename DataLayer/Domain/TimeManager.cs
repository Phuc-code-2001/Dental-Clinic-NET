using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Domain
{

    public class TimeManager
    {
        private static TimeManager _instance;

        public static TimeManager Instance { get
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
            return String.Format("{0}h{1}m", duration.Hours.ToString("00"), duration.Minutes.ToString("00"));
        }

        public string TryConvertToStrTime(SlotManager slot)
        {
            var time = GetTime(slot);
            return time.HasValue ? ConvertToStrTime(time.Value) : "ambiguous!";
        }
    }
}