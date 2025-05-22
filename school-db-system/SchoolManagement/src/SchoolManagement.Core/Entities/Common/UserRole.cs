using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Common
{
    /// <summary>
    /// Represents the association between a user and a role within a tenant
    /// </summary>
    [Table("user_roles", Schema = "common")]
    public class UserRole
    {
        [Key, Column(Order = 0)]
        public string UserId { get; set; }
        
        [Key, Column(Order = 1)]
        public string RoleId { get; set; }
        
        public string TenantId { get; set; }
        
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
