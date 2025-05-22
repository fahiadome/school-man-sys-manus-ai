using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Tenant
{
    /// <summary>
    /// Represents a term/semester within an academic year
    /// </summary>
    [Table("terms")]
    public class Term : BaseEntity
    {
        [Required]
        public string AcademicYearId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        public bool IsCurrent { get; set; }
        
        [ForeignKey("AcademicYearId")]
        public virtual AcademicYear AcademicYear { get; set; }
    }
}
