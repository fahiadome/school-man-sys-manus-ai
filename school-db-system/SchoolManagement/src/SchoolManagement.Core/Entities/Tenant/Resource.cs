using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Tenant
{
    /// <summary>
    /// Represents a resource in the system
    /// </summary>
    [Table("resources")]
    public class Resource : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(50)]
        public string ResourceType { get; set; }  // Textbook, Equipment, Furniture, etc.
        
        public int Quantity { get; set; }
        
        public int AvailableQuantity { get; set; }
        
        [StringLength(100)]
        public string Location { get; set; }
        
        public DateTime? AcquisitionDate { get; set; }
        
        [StringLength(50)]
        public string Condition { get; set; }  // New, Good, Fair, Poor
    }
}
