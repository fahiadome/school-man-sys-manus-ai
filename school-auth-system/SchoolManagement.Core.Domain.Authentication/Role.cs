using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Core.Domain.Authentication
{
    /// <summary>
    /// Role entity for authorization
    /// </summary>
    public class Role
    {
        /// <summary>
        /// Gets or sets the role ID
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// Gets or sets the role name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the role description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a system role
        /// </summary>
        public bool IsSystemRole { get; set; }

        /// <summary>
        /// Gets or sets the creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last update date
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the user roles
        /// </summary>
        public virtual ICollection<UserRole> UserRoles { get; set; }

        /// <summary>
        /// Gets or sets the role permissions
        /// </summary>
        public virtual ICollection<RolePermission> RolePermissions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Role"/> class
        /// </summary>
        public Role()
        {
            UserRoles = new List<UserRole>();
            RolePermissions = new List<RolePermission>();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Role"/> class
        /// </summary>
        public Role(string name, string description = null, bool isSystemRole = false)
            : this()
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            IsSystemRole = isSystemRole;
        }

        /// <summary>
        /// Updates the role
        /// </summary>
        public void Update(string name, string description)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Adds a permission to the role
        /// </summary>
        public void AddPermission(string permissionId)
        {
            if (string.IsNullOrEmpty(permissionId))
                throw new ArgumentNullException(nameof(permissionId));

            RolePermissions.Add(new RolePermission
            {
                RoleId = Id,
                PermissionId = permissionId
            });
            
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Removes a permission from the role
        /// </summary>
        public void RemovePermission(string permissionId)
        {
            if (string.IsNullOrEmpty(permissionId))
                throw new ArgumentNullException(nameof(permissionId));

            var permission = RolePermissions.FirstOrDefault(rp => rp.PermissionId == permissionId);
            
            if (permission != null)
            {
                RolePermissions.Remove(permission);
                UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
