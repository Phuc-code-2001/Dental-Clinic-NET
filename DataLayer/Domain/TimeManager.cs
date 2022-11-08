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

    public class TimeManager : Dictionary<TimeManager.Slot, Nullable<TimeSpan>>
    {
        private static TimeManager _instance;

        public static TimeManager Instance { get
            {
                return _instance ??= new TimeManager();
            }
        }

        public enum Slot
        {
            Slot_01,
            Slot_02,
            Slot_03,
            Slot_04,
            Slot_05,
            Slot_06,
        }

        private TimeManager() : base()
        {

            this.Add(Slot.Slot_01, new TimeSpan(8, 0, 0));

            this.Add(Slot.Slot_02, new TimeSpan(9, 0, 0));

            this.Add(Slot.Slot_03, new TimeSpan(10, 0, 0));

            this.Add(Slot.Slot_04, new TimeSpan(14, 0, 0));

            this.Add(Slot.Slot_05, new TimeSpan(15, 0, 0));

            this.Add(Slot.Slot_06, new TimeSpan(16, 0, 0));

        }
        
    }
}