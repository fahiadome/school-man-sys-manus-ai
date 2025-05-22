using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Tenant
{
    /// <summary>
    /// Represents the association between a teacher and a subject
    /// </summary>
    [Table("teacher_subjects")]
    public class TeacherSubject
    {
        [Key, Column(Order = 0)]
        public string TeacherId { get; set; }
        
        [Key, Column(Order = 1)]
        public string SubjectId { get; set; }
        
        public string AcademicYearId { get; set; }
        
        [ForeignKey("TeacherId")]
        public virtual Staff Teacher { get; set; }
        
        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; }
        
        [ForeignKey("AcademicYearId")]
        public virtual AcademicYear AcademicYear { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
