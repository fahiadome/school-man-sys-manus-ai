using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;
using SchoolManagement.Core.Entities.Master;

namespace SchoolManagement.Core.Entities.Common
{
    /// <summary>
    /// Represents the association between a user and a tenant
    /// </summary>
    [Table("user_tenants", Schema = "common")]
    public class UserTenant
    {
        [Key, Column(Order = 0)]
        public string UserId { get; set; }
        
        [Key, Column(Order = 1)]
        public string TenantId { get; set; }
        
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
        
        public bool IsPrimary { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
