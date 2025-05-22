using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Master
{
    /// <summary>
    /// Represents a subscription for a tenant
    /// </summary>
    [Table("tenant_subscriptions", Schema = "master")]
    public class TenantSubscription : BaseEntity
    {
        [Required]
        public string TenantId { get; set; }
        
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
        
        [Required]
        public string PlanId { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public bool IsTrial { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; }
        
        [Required]
        [StringLength(20)]
        public string PaymentStatus { get; set; }
    }
}
