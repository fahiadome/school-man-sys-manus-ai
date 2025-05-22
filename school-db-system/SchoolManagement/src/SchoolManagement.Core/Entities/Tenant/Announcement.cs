using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Tenant
{
    /// <summary>
    /// Represents an announcement in the system
    /// </summary>
    [Table("announcements")]
    public class Announcement : BaseEntity
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        
        [Required]
        public string Content { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        [Required]
        [StringLength(50)]
        public string AudienceType { get; set; }  // All, Staff, Students, Parents, Grade, Section
        
        public int? GradeId { get; set; }
        
        public int? SectionId { get; set; }
        
        public int CreatedBy { get; set; }  // User ID of the staff who created this
        
        [ForeignKey("GradeId")]
        public virtual Grade Grade { get; set; }
        
        [ForeignKey("SectionId")]
        public virtual Section Section { get; set; }
    }
}
