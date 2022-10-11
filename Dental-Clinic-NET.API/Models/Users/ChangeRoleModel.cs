using DataLayer.Domain;
using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.Models.Users
{
    public class ChangeRoleModel
    {
        [Required]
        public string UserId { get; set; }
        [Range(0, 4)]
        public UserType RoleId { get; set; }
    }
}
