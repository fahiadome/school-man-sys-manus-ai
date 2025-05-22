using System;

namespace SchoolManagement.Infrastructure.Multitenancy
{
    /// <summary>
    /// Interface for tenant information
    /// </summary>
    public interface ITenantInfo
    {
        /// <summary>
        /// Gets the tenant ID
        /// </summary>
        int Id { get; }
        
        /// <summary>
        /// Gets the tenant name
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Gets the schema name for the tenant
        /// </summary>
        string SchemaName { get; }
        
        /// <summary>
        /// Gets the connection string for the tenant
        /// </summary>
        string ConnectionString { get; }
        
        /// <summary>
        /// Gets whether the tenant is active
        /// </summary>
        bool IsActive { get; }
    }
}
