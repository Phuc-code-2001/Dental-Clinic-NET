﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Domain
{
    
    public class UserLock : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(BaseUser))]
        public string BaseUserId { get; set; }

        public string Reason { get; set; }
        public bool IsLocked { get; set; }
        public DateTime Expired { get; set; }


        public bool IsLockCalculated => (IsLocked && Expired >= DateTime.Now);
    }
}
