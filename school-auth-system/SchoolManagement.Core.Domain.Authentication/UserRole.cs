using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Core.Domain.Authentication
{
    /// <summary>
    /// User-role association entity
    /// </summary>
    public class UserRole
    {
        /// <summary>
        /// Gets or sets the user ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the role ID
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// Gets or sets the tenant ID (optional, for tenant-specific roles)
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets the creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the user
        /// </summary>
        public virtual AuthUser User { get; set; }

        /// <summary>
        /// Gets or sets the role
        /// </summary>
        public virtual Role Role { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRole"/> class
        /// </summary>
        public UserRole()
        {
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRole"/> class
        /// </summary>
        public UserRole(string userId, string roleId, string tenantId = null)
            : this()
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            RoleId = roleId ?? throw new ArgumentNullException(nameof(roleId));
            TenantId = tenantId;
        }
    }
}
