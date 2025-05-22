using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Core.Domain.Authentication
{
    /// <summary>
    /// Role-permission association entity
    /// </summary>
    public class RolePermission
    {
        /// <summary>
        /// Gets or sets the role ID
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// Gets or sets the permission ID
        /// </summary>
        public string PermissionId { get; set; }

        /// <summary>
        /// Gets or sets the creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the role
        /// </summary>
        public virtual Role Role { get; set; }

        /// <summary>
        /// Gets or sets the permission
        /// </summary>
        public virtual Permission Permission { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RolePermission"/> class
        /// </summary>
        public RolePermission()
        {
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RolePermission"/> class
        /// </summary>
        public RolePermission(string roleId, string permissionId)
            : this()
        {
            RoleId = roleId ?? throw new ArgumentNullException(nameof(roleId));
            PermissionId = permissionId ?? throw new ArgumentNullException(nameof(permissionId));
        }
    }
}
