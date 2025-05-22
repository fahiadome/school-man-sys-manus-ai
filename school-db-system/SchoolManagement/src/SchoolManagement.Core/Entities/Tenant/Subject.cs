using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Tenant
{
    /// <summary>
    /// Represents a subject in the system
    /// </summary>
    [Table("subjects")]
    public class Subject : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Code { get; set; }
        
        [StringLength(255)]
        public string Description { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
