using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Schemas
{
    [Table("GroupMembers")]
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
