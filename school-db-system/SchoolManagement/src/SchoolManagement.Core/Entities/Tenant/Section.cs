using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Tenant
{
    /// <summary>
    /// Represents a section/division within a grade
    /// </summary>
    [Table("sections")]
    public class Section : BaseEntity
    {
        [Required]
        public string GradeId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        
        public int Capacity { get; set; }
        
        [StringLength(50)]
        public string Room { get; set; }
        
        [ForeignKey("GradeId")]
        public virtual Grade Grade { get; set; }
    }
}
