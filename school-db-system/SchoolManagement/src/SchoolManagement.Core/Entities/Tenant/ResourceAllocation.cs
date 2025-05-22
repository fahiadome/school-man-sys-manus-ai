using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Tenant
{
    /// <summary>
    /// Represents a resource allocation to a user or class
    /// </summary>
    [Table("resource_allocations")]
    public class ResourceAllocation : BaseEntity
    {
        [Required]
        public int ResourceId { get; set; }
        
        [Required]
        public int AllocatedTo { get; set; }  // User ID or Section ID
        
        [Required]
        [StringLength(20)]
        public string AllocationType { get; set; }  // Student, Teacher, Staff, Section
        
        [Required]
        public DateTime AllocationDate { get; set; }
        
        public DateTime? ReturnDate { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; }  // Allocated, Returned, Damaged, Lost
        
        [ForeignKey("ResourceId")]
        public virtual Resource Resource { get; set; }
    }
}
