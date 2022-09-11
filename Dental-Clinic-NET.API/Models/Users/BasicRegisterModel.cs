using DataLayer.Schemas;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.Models.Users
{
    public class BasicRegisterModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }

        public DateTime BirthDate { get; set; } = DateTime.Now;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string Gender { get; set; }

        public BaseUser ToBaseUser_NotIncludePassword()
        {
            return new BaseUser()
            {
                UserName = UserName,
                FullName = FullName,
                Email = Email,
                Gender = Gender,
                PhoneNumber = PhoneNumber,
                BirthDate = BirthDate,
            };
        }
    }
}
