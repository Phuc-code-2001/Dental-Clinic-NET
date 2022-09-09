using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Schemas
{
    public class BaseUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; } = "Undefined";
        public string ImageURL { get; set; } = "https://ik.imagekit.io/sdrpji7cj/default-image.jpg?ik-sdk-version=javascript-1.4.3&updatedAt=1658454695102";

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; } = DateTime.Now.Date;

        public string Gender { get; set; }

        public UserType Type { get; set; } = UserType.Patient;

        public string FbConnectedId { get; set; }

    }

    public enum UserType
    {
        Patient,
        Doctor,
        Receptionist,
        Technical,
        Administrator,
    }
}
