using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Tenant
{
    /// <summary>
    /// Represents a fee type in the system
    /// </summary>
    [Table("fee_types")]
    public class FeeType : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(255)]
        public string Description { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Frequency { get; set; }  // One-time, Term, Annual
        
        public bool IsMandatory { get; set; }
    }
}
