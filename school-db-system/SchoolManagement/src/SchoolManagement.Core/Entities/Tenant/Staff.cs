using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Tenant
{
    /// <summary>
    /// Represents a staff member in the system
    /// </summary>
    [Table("staff")]
    public class Staff : BaseEntity
    {
        [Required]
        public string UserId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string EmployeeId { get; set; }
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        [StringLength(10)]
        public string Gender { get; set; }
        
        [StringLength(255)]
        public string Address { get; set; }
        
        [Required]
        public DateTime JoiningDate { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Designation { get; set; }
        
        [StringLength(100)]
        public string Department { get; set; }
        
        [StringLength(255)]
        public string Qualification { get; set; }
        
        public int Experience { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active";
        
        [StringLength(255)]
        public string PhotoUrl { get; set; }
        
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
