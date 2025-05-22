using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Tenant
{
    /// <summary>
    /// Represents an assessment result for a student
    /// </summary>
    [Table("assessment_results")]
    public class AssessmentResult : BaseEntity
    {
        [Required]
        public int AssessmentId { get; set; }
        
        [Required]
        public int StudentId { get; set; }
        
        [Required]
        public decimal Score { get; set; }
        
        [StringLength(20)]
        public string Grade { get; set; }
        
        [StringLength(255)]
        public string Remarks { get; set; }
        
        public int RecordedBy { get; set; }  // User ID of the staff who recorded this
        
        [ForeignKey("AssessmentId")]
        public virtual Assessment Assessment { get; set; }
        
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
    }
}
