﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.Models.Users
{
    public class UpdateUserModel
    {
        [Required]
        public string userId { get; set; }
        public string FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }

    }
}