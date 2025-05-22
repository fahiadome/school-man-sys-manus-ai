using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Core.Entities.Common;

namespace SchoolManagement.Core.Entities.Common
{
    /// <summary>
    /// Represents a permission in the system
    /// </summary>
    [Table("permissions", Schema = "common")]
    public class Permission : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(255)]
        public string Description { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Resource { get; set; }
    }
}
