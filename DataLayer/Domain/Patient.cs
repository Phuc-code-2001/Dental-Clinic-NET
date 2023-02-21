using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Domain
{
    [Table("Patients")]
    public class Patient : BaseEntity
    {
        [Key]
        [ForeignKey("BaseUser")]
        public string Id { get; set; }
        public BaseUser BaseUser { get; set; }

        [ForeignKey("FileId")]
        public FileMedia MedicalRecordFile { get; set; }
        [ForeignKey("MediaFile")]
        public int FileId { get; set; }
    }
}
