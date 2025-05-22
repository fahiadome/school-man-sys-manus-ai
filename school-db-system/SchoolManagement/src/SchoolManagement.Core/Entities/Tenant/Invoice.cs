using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Tenant
{
    /// <summary>
    /// Represents an invoice for a student
    /// </summary>
    [Table("invoices")]
    public class Invoice : BaseEntity
    {
        [Required]
        public string StudentId { get; set; }
        
        [Required]
        public string AcademicYearId { get; set; }
        
        public string TermId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string InvoiceNumber { get; set; }
        
        [Required]
        public DateTime IssueDate { get; set; }
        
        [Required]
        public DateTime DueDate { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal NetAmount { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Status { get; set; }  // Pending, Paid, Partially Paid, Overdue
        
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
        
        [ForeignKey("AcademicYearId")]
        public virtual AcademicYear AcademicYear { get; set; }
        
        [ForeignKey("TermId")]
        public virtual Term Term { get; set; }
    }
}
