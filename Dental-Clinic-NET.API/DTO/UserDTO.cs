using DataLayer.Domain;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.DTO
{
    public class UserDTO
    {
        [Key]
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("fullname")]
        public string FullName { get; set; }

        [JsonProperty("image_url")]
        public string ImageURL { get; set; }

        [JsonProperty("birthday")]
        public DateTime BirthDate { get; set; }

        [JsonProperty("id")]
        public string Gender { get; set; }

        [JsonProperty("phone")]
        public string PhoneNumber { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("facebook_id")]
        public string FbConnectedId { get; set; }

    }
}
