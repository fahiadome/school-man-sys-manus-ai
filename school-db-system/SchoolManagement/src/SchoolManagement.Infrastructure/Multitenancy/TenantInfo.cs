using System;

namespace SchoolManagement.Infrastructure.Multitenancy
{
    /// <summary>
    /// Implementation of tenant information
    /// </summary>
    public class TenantInfo : ITenantInfo
    {
        /// <summary>
        /// Gets the tenant ID
        /// </summary>
        public int Id { get; private set; }
        
        /// <summary>
        /// Gets the tenant name
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// Gets the schema name for the tenant
        /// </summary>
        public string SchemaName { get; private set; }
        
        /// <summary>
        /// Gets the connection string for the tenant
        /// </summary>
        public string ConnectionString { get; private set; }
        
        /// <summary>
        /// Gets whether the tenant is active
        /// </summary>
        public bool IsActive { get; private set; }
        
        /// <summary>
        /// Creates a new instance of TenantInfo
        /// </summary>
        public TenantInfo(int id, string name, string schemaName, string connectionString, bool isActive)
        {
            Id = id;
            Name = name;
            SchemaName = schemaName;
            ConnectionString = connectionString;
            IsActive = isActive;
        }
    }
}
