using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Tenant
{
    /// <summary>
    /// Represents a student enrollment record
    /// </summary>
    [Table("enrollments")]
    public class Enrollment : BaseEntity
    {
        [Required]
        public string StudentId { get; set; }
        
        [Required]
        public string AcademicYearId { get; set; }
        
        [Required]
        public string TermId { get; set; }
        
        [Required]
        public string GradeId { get; set; }
        
        public string SectionId { get; set; }
        
        [Required]
        public DateTime EnrollmentDate { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active";
        
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
        
        [ForeignKey("AcademicYearId")]
        public virtual AcademicYear AcademicYear { get; set; }
        
        [ForeignKey("TermId")]
        public virtual Term Term { get; set; }
        
        [ForeignKey("GradeId")]
        public virtual Grade Grade { get; set; }
        
        [ForeignKey("SectionId")]
        public virtual Section Section { get; set; }
    }
}
