using DataLayer.Domain;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.DTOs
{
    public class UserDTO
    {
        [Key]
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }

        public string FullName { get; set; }

        public string ImageURL { get; set; }

        public DateTime BirthDate { get; set; }

        public string Gender { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }

        public string Role { get; set; }

        public string PusherChannel { get; set; }

        public bool IsLock { get; set; }

    }

    public class UserDTOLite
    {
        [Key]
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string ImageURL { get; set; }

    }
}
