using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Master
{
    /// <summary>
    /// Represents a tenant (school) in the system
    /// </summary>
    [Table("tenants", Schema = "master")]
    public class Tenant : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(63)]
        public string SchemaName { get; set; }
        
        [StringLength(100)]
        public string Domain { get; set; }
        
        [StringLength(255)]
        public string ConnectionString { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        [Column(TypeName = "jsonb")]
        public string Settings { get; set; }
    }
}
