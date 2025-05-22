using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Tenant
{
    /// <summary>
    /// Represents a grade/class level in the system
    /// </summary>
    [Table("grades")]
    public class Grade : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(50)]
        public string DisplayName { get; set; }
        
        [Required]
        public int Sequence { get; set; }
    }
}
