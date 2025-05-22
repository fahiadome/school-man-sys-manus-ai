using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Core.Entities.Common
{
    /// <summary>
    /// Base entity for all entities with common properties
    /// </summary>
    public abstract class BaseEntity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
