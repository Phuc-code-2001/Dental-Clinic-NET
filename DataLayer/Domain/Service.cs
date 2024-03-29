﻿using DataLayer.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DataLayer.Domain
{
    public class Service : BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }

        public string ImageURL { get; set; }
        public string ImageId { get; set; }

        public string Description { get; set; }
        public int Price { get; set; }

        public bool IsPublic { get; set; } = true;

        public ICollection<Device> Devices { get; set; }
        public List<Post> Posts { get; set; }

    }
}
