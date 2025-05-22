using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Common
{
    /// <summary>
    /// Represents the association between a role and a permission
    /// </summary>
    [Table("role_permissions", Schema = "common")]
    public class RolePermission
    {
        [Key, Column(Order = 0)]
        public string RoleId { get; set; }
        
        [Key, Column(Order = 1)]
        public string PermissionId { get; set; }
        
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
        
        [ForeignKey("PermissionId")]
        public virtual Permission Permission { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
