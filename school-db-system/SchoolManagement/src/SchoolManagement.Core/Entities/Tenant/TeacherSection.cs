using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Tenant
{
    /// <summary>
    /// Represents the association between a teacher and a section
    /// </summary>
    [Table("teacher_sections")]
    public class TeacherSection
    {
        [Key, Column(Order = 0)]
        public string TeacherId { get; set; }
        
        [Key, Column(Order = 1)]
        public string SectionId { get; set; }
        
        public string AcademicYearId { get; set; }
        
        public bool IsClassTeacher { get; set; }
        
        [ForeignKey("TeacherId")]
        public virtual Staff Teacher { get; set; }
        
        [ForeignKey("SectionId")]
        public virtual Section Section { get; set; }
        
        [ForeignKey("AcademicYearId")]
        public virtual AcademicYear AcademicYear { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
