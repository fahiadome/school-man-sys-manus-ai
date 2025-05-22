using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Tenant
{
    /// <summary>
    /// Represents a student in the system
    /// </summary>
    [Table("students")]
    public class Student : BaseEntity
    {
        [Required]
        public string UserId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string AdmissionNumber { get; set; }
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        [StringLength(10)]
        public string Gender { get; set; }
        
        [StringLength(255)]
        public string Address { get; set; }
        
        [StringLength(50)]
        public string Nationality { get; set; }
        
        [StringLength(50)]
        public string Religion { get; set; }
        
        [Required]
        public DateTime AdmissionDate { get; set; }
        
        public string CurrentGradeId { get; set; }
        
        public string CurrentSectionId { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active";
        
        [StringLength(255)]
        public string PhotoUrl { get; set; }
        
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        
        [ForeignKey("CurrentGradeId")]
        public virtual Grade CurrentGrade { get; set; }
        
        [ForeignKey("CurrentSectionId")]
        public virtual Section CurrentSection { get; set; }
    }
}
