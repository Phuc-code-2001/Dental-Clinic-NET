using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DataLayer.Domain
{
    public class Doctor :  BaseEntity
    {
        [Key]
        [ForeignKey("BaseUser")]
        public string Id { get; set; }
        public BaseUser BaseUser { get; set; }

        public Majors Major { get; set; } = Majors.Unknown;
        
        public FileMedia Certificate { get; set; }

        public bool Verified { get; set; }

        public enum Majors
        {
            Unknown,
            Allergist,
            Andrologist,
            Cardiologist,
            Endocrinologist,
            Gastroenterologist,
        }
    }
    
}
