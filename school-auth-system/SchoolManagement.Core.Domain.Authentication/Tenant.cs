using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Core.Domain.Authentication
{
    /// <summary>
    /// Tenant entity for multi-tenancy
    /// </summary>
    public class Tenant
    {
        /// <summary>
        /// Gets or sets the tenant ID
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// Gets or sets the tenant name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the schema name
        /// </summary>
        public string SchemaName { get; set; }

        /// <summary>
        /// Gets or sets the tenant domain
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tenant is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the subscription expiry date
        /// </summary>
        public DateTime? SubscriptionExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last update date
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tenant"/> class
        /// </summary>
        public Tenant()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tenant"/> class
        /// </summary>
        public Tenant(string name, string schemaName, string domain = null)
            : this()
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            SchemaName = schemaName ?? throw new ArgumentNullException(nameof(schemaName));
            Domain = domain;
        }

        /// <summary>
        /// Updates the tenant
        /// </summary>
        public void Update(string name, string domain)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Domain = domain;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Activates the tenant
        /// </summary>
        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Deactivates the tenant
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Sets the subscription expiry date
        /// </summary>
        public void SetSubscriptionExpiryDate(DateTime expiryDate)
        {
            SubscriptionExpiryDate = expiryDate;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
