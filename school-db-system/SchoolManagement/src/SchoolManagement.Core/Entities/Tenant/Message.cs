using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Tenant
{
    /// <summary>
    /// Represents a direct message between users
    /// </summary>
    [Table("messages")]
    public class Message : BaseEntity
    {
        [Required]
        public int SenderId { get; set; }
        
        [Required]
        public int RecipientId { get; set; }
        
        [StringLength(200)]
        public string Subject { get; set; }
        
        [Required]
        public string Content { get; set; }
        
        public bool IsRead { get; set; } = false;
        
        public DateTime? ReadAt { get; set; }
    }
}
