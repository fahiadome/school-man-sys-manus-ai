using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Tenant
{
    /// <summary>
    /// Represents a payment for an invoice
    /// </summary>
    [Table("payments")]
    public class Payment : BaseEntity
    {
        [Required]
        public string InvoiceId { get; set; }
        
        [Required]
        public DateTime PaymentDate { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; }  // Cash, Mobile Money, Bank Transfer, Check
        
        [StringLength(100)]
        public string ReferenceNumber { get; set; }
        
        public string ReceivedBy { get; set; }  // User ID of the staff who received this payment
        
        [StringLength(255)]
        public string Remarks { get; set; }
        
        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }
    }
}
