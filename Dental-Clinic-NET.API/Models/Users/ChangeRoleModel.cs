using System;
using DataLayer.Domain;
using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.Models.Users
{
    public class ChangeRoleModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        [RangeRole]
        public UserType RoleId { get; set; }

        private class RangeRoleAttribute : ValidationAttribute
        {

            private int _min;
            private int _max;
            private string _message;

            public RangeRoleAttribute() 
            {
                _min = 0;
                _max = Enum.GetValues<UserType>().Length;
                _message = $"This value out of range, accept in range ({_min}, {_max - 1})";
            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if((int) value < _min || (int) value >= _max)
                {
                    return new ValidationResult(_message);
                }

                return ValidationResult.Success;
            }
        }
    }
}
