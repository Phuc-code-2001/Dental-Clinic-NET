using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Domain
{
    public class BaseUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; } = "Undefined";
        public string ImageURL { get; set; } = "https://ik.imagekit.io/sdrpji7cj/default-image.jpg?ik-sdk-version=javascript-1.4.3&updatedAt=1658454695102";
        public string ImageAvatarId { get; set; }

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; } = DateTime.Now.Date;

        public string Gender { get; set; }

        public string Address { get; set; }

        public UserType Type { get; set; } = UserType.Patient;

        public string PusherChannel { get; set; }

        // Relationship
        public EmailConfirmation EmailConfirmation { get; set; }
        public List<UserLock> UserLocks { get; set; }

    }

    public enum UserType
    {
        Patient,
        Doctor,
        Receptionist,
        Technician,
        Expert,
        Administrator,
    }
}
