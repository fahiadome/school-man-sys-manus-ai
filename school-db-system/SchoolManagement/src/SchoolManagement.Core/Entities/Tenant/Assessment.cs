using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Tenant
{
    /// <summary>
    /// Represents an assessment in the system
    /// </summary>
    [Table("assessments")]
    public class Assessment : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(255)]
        public string Description { get; set; }
        
        [Required]
        [StringLength(50)]
        public string AssessmentType { get; set; }  // Quiz, Test, Exam, Project, etc.
        
        [Required]
        public int TermId { get; set; }
        
        [Required]
        public int GradeId { get; set; }
        
        [Required]
        public int SubjectId { get; set; }
        
        [Required]
        public decimal MaxScore { get; set; }
        
        public decimal PassingScore { get; set; }
        
        public DateTime AssessmentDate { get; set; }
        
        public int CreatedBy { get; set; }  // User ID of the staff who created this
        
        [ForeignKey("TermId")]
        public virtual Term Term { get; set; }
        
        [ForeignKey("GradeId")]
        public virtual Grade Grade { get; set; }
        
        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; }
    }
}
