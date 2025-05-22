using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Tenant
{
    /// <summary>
    /// Represents an invoice item for a specific fee
    /// </summary>
    [Table("invoice_items")]
    public class InvoiceItem : BaseEntity
    {
        [Required]
        public string InvoiceId { get; set; }
        
        [Required]
        public string FeeStructureId { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal NetAmount { get; set; }
        
        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }
        
        [ForeignKey("FeeStructureId")]
        public virtual FeeStructure FeeStructure { get; set; }
    }
}
