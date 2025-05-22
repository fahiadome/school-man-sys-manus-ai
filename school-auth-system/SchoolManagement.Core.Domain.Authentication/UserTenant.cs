using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Core.Domain.Authentication
{
    /// <summary>
    /// User-tenant association entity
    /// </summary>
    public class UserTenant
    {
        /// <summary>
        /// Gets or sets the user ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the tenant ID
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is the primary tenant for the user
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Gets or sets the creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the user
        /// </summary>
        public virtual AuthUser User { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserTenant"/> class
        /// </summary>
        public UserTenant()
        {
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserTenant"/> class
        /// </summary>
        public UserTenant(string userId, string tenantId, bool isPrimary = false)
            : this()
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
            IsPrimary = isPrimary;
        }
    }
}
