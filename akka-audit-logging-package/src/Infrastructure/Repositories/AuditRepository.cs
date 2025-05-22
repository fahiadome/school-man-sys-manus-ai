using System;
using System.Threading.Tasks;
using SchoolManagement.Domain.Entities.Audit;
using SchoolManagement.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace SchoolManagement.Infrastructure.Repositories
{
    /// <summary>
    /// Implementation of the audit repository
    /// </summary>
    public class AuditRepository : IAuditRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AuditRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Save a batch of audit records
        /// </summary>
        public async Task SaveAuditBatchAsync(IEnumerable<Audit> auditEntities)
        {
            try
            {
                // Add all entities to the context
                await _dbContext.Audits.AddRangeAsync(auditEntities);
                
                // Save changes
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log error and rethrow
                // In a real implementation, you might want to add logging here
                throw new Exception("Error saving audit batch", ex);
            }
        }

        /// <summary>
        /// Get audit trail for an entity
        /// </summary>
        public async Task<IEnumerable<AuditRecord>> GetAuditTrailAsync(
            string entityName, 
            string entityId, 
            string tenantId,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int pageSize = 100,
            int pageNumber = 1)
        {
            try
            {
                // Build query
                var query = _dbContext.Audits
                    .Where(a => a.EntityName == entityName && a.EntityId == entityId && a.TenantId == tenantId);
                
                // Apply date filters if provided
                if (startDate.HasValue)
                {
                    query = query.Where(a => a.Timestamp >= startDate.Value);
                }
                
                if (endDate.HasValue)
                {
                    query = query.Where(a => a.Timestamp <= endDate.Value);
                }
                
                // Apply paging
                var skip = (pageNumber - 1) * pageSize;
                
                // Execute query
                var audits = await query
                    .OrderByDescending(a => a.Timestamp)
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync();
                
                // Map to audit records
                return audits.Select(a => new AuditRecord
                {
                    Id = a.Id,
                    TenantId = a.TenantId,
                    EntityName = a.EntityName,
                    EntityId = a.EntityId,
                    Action = a.Action,
                    UserId = a.UserId,
                    Timestamp = a.Timestamp,
                    OriginalValues = a.OriginalValues,
                    NewValues = a.NewValues,
                    ProcessedAt = a.ProcessedAt
                });
            }
            catch (Exception ex)
            {
                // Log error and rethrow
                throw new Exception("Error retrieving audit trail", ex);
            }
        }

        /// <summary>
        /// Get total count of audit records for an entity
        /// </summary>
        public async Task<int> GetAuditTrailCountAsync(
            string entityName, 
            string entityId, 
            string tenantId,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            try
            {
                // Build query
                var query = _dbContext.Audits
                    .Where(a => a.EntityName == entityName && a.EntityId == entityId && a.TenantId == tenantId);
                
                // Apply date filters if provided
                if (startDate.HasValue)
                {
                    query = query.Where(a => a.Timestamp >= startDate.Value);
                }
                
                if (endDate.HasValue)
                {
                    query = query.Where(a => a.Timestamp <= endDate.Value);
                }
                
                // Execute count query
                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                // Log error and rethrow
                throw new Exception("Error counting audit records", ex);
            }
        }
    }
}
