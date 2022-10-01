using DataLayer.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.Models.Contacts
{
    public class UpdateContact
    {
        public int Id { get; set; }

        [Range(0, 2)]
        public ContactStates StateIndex { get; set; }
    }
}
