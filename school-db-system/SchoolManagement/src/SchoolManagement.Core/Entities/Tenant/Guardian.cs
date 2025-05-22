using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Tenant
{
    /// <summary>
    /// Represents a guardian (parent) in the system
    /// </summary>
    [Table("guardians")]
    public class Guardian : BaseEntity
    {
        [Required]
        public string UserId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Relationship { get; set; }
        
        [StringLength(100)]
        public string Occupation { get; set; }
        
        [StringLength(255)]
        public string WorkAddress { get; set; }
        
        [StringLength(50)]
        public string EducationLevel { get; set; }
        
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
