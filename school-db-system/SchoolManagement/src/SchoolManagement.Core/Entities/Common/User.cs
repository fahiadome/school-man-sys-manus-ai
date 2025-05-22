using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Common
{
    /// <summary>
    /// Represents a user in the system
    /// </summary>
    [Table("users", Schema = "common")]
    public class User : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }
        
        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; }
        
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        
        [StringLength(20)]
        public string PhoneNumber { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime? LastLogin { get; set; }
    }
}
