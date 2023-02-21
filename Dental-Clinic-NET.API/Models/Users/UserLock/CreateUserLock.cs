using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.Models.Users.UserLock
{
    
    public class CreateUserLock
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Reason { get; set; }

        [Required]
        public DateTime Expired { get; set; }

    }
}
