using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Schemas
{
    [Table("GroupMembers")]
    [Index(nameof(MemberCode), IsUnique = true)] // Setup để bảo vệ database, ko phải để validate dữ liệu phía client
    public class GroupMember
    {
        [Key]
        public int Id { get; set; }

        [StringLength(maximumLength: 8)]
        public string MemberCode { get; set; }

        [StringLength(maximumLength: 32)]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime BirthDay { get; set; }

        public string Description { get; set; }
    }
}
