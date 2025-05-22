using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Tenant
{
    /// <summary>
    /// Represents a fee structure for a specific grade, academic year, and term
    /// </summary>
    [Table("fee_structures")]
    public class FeeStructure : BaseEntity
    {
        [Required]
        public string FeeTypeId { get; set; }
        
        [Required]
        public string GradeId { get; set; }
        
        [Required]
        public string AcademicYearId { get; set; }
        
        public string TermId { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        public DateTime? DueDate { get; set; }
        
        [ForeignKey("FeeTypeId")]
        public virtual FeeType FeeType { get; set; }
        
        [ForeignKey("GradeId")]
        public virtual Grade Grade { get; set; }
        
        [ForeignKey("AcademicYearId")]
        public virtual AcademicYear AcademicYear { get; set; }
        
        [ForeignKey("TermId")]
        public virtual Term Term { get; set; }
    }
}
