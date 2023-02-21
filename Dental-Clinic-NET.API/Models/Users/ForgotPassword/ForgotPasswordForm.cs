using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.Models.Users.ForgotPassword
{
    public class ForgotPasswordForm
    {
        [Required]
        public string UserName { get; set; }
    }
}
