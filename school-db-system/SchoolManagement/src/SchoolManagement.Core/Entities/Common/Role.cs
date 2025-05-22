using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Common
{
    /// <summary>
    /// Represents a role in the system
    /// </summary>
    [Table("roles", Schema = "common")]
    public class Role : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        
        [StringLength(255)]
        public string Description { get; set; }
        
        public bool IsSystemRole { get; set; }
    }
}
