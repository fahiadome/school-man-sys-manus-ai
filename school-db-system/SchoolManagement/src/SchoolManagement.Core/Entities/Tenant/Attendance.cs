using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Tenant
{
    /// <summary>
    /// Represents a student attendance record
    /// </summary>
    [Table("attendance")]
    public class Attendance : BaseEntity
    {
        [Required]
        public string StudentId { get; set; }
        
        [Required]
        public DateTime Date { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; }  // Present, Absent, Late, Excused
        
        [StringLength(255)]
        public string Reason { get; set; }
        
        public string RecordedBy { get; set; }  // User ID of the staff who recorded this
        
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
    }
}
