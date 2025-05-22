using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Tenant
{
    /// <summary>
    /// Represents an academic year in the system
    /// </summary>
    [Table("academic_years")]
    public class AcademicYear : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        public bool IsCurrent { get; set; }
    }
}
