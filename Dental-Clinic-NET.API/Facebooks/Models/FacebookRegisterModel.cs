using DataLayer.Schemas;
using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.Facebooks.Models
{
    public class FacebookRegisterModel
    {
        public string AccessToken { get; set; }

        [Required]
        public string UserName { get; set; }


    }
}
