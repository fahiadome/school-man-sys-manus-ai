using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Tenant
{
    /// <summary>
    /// Represents the association between a student and a guardian
    /// </summary>
    [Table("student_guardians")]
    public class StudentGuardian
    {
        [Key, Column(Order = 0)]
        public string StudentId { get; set; }
        
        [Key, Column(Order = 1)]
        public string GuardianId { get; set; }
        
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
        
        [ForeignKey("GuardianId")]
        public virtual Guardian Guardian { get; set; }
        
        public bool IsPrimary { get; set; }
        
        public bool CanPickup { get; set; }
        
        public bool EmergencyContact { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
