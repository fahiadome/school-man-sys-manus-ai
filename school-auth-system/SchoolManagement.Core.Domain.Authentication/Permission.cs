using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Core.Domain.Authentication
{
    /// <summary>
    /// Permission entity for fine-grained access control
    /// </summary>
    public class Permission
    {
        /// <summary>
        /// Gets or sets the permission ID
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// Gets or sets the permission name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the permission description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the resource this permission applies to
        /// </summary>
        public string Resource { get; set; }

        /// <summary>
        /// Gets or sets the creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last update date
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Permission"/> class
        /// </summary>
        public Permission()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Permission"/> class
        /// </summary>
        public Permission(string name, string resource, string description = null)
            : this()
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Resource = resource ?? throw new ArgumentNullException(nameof(resource));
            Description = description;
        }

        /// <summary>
        /// Updates the permission
        /// </summary>
        public void Update(string name, string resource, string description)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Resource = resource ?? throw new ArgumentNullException(nameof(resource));
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
