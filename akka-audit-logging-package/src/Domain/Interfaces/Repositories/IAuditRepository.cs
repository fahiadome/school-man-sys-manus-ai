using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolManagement.Domain.Entities.Audit;

namespace SchoolManagement.Infrastructure.Actors.Audit
{
    /// <summary>
    /// Repository interface for audit operations
    /// </summary>
    public interface IAuditRepository
    {
        /// <summary>
        /// Save a batch of audit records
        /// </summary>
        /// <param name="auditEntities">Collection of audit entities to save</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task SaveAuditBatchAsync(IEnumerable<Audit> auditEntities);
        
        /// <summary>
        /// Get audit trail for an entity
        /// </summary>
        /// <param name="entityName">Entity name</param>
        /// <param name="entityId">Entity ID</param>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="startDate">Optional start date filter</param>
        /// <param name="endDate">Optional end date filter</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <returns>Collection of audit records</returns>
        Task<IEnumerable<AuditRecord>> GetAuditTrailAsync(
            string entityName, 
            string entityId, 
            string tenantId,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int pageSize = 100,
            int pageNumber = 1);
            
        /// <summary>
        /// Get total count of audit records for an entity
        /// </summary>
        /// <param name="entityName">Entity name</param>
        /// <param name="entityId">Entity ID</param>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="startDate">Optional start date filter</param>
        /// <param name="endDate">Optional end date filter</param>
        /// <returns>Total count of audit records</returns>
        Task<int> GetAuditTrailCountAsync(
            string entityName, 
            string entityId, 
            string tenantId,
            DateTime? startDate = null,
            DateTime? endDate = null);
    }
}
