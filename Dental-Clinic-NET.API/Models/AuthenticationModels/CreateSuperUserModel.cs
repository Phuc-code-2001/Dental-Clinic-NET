using DataLayer.Schemas;
using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.Models.AuthenticationModels
{
    public class CreateSuperUserModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        public string FullName { get; set; }
        public string ImageURL { get; set; }

        public BaseUser ToBaseUser_NotIncludePassword()
        {
            BaseUser user = new BaseUser
            {
                UserName = UserName,
                Email = Email,
                PhoneNumber = PhoneNumber,
                FullName = FullName,
                ImageURL = ImageURL,
            };

            return user;
        }
    }
}
