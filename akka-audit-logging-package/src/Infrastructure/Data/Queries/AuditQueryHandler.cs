using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities.Audit;
using SchoolManagement.Infrastructure.Actors.Audit;

namespace SchoolManagement.Infrastructure.Data.Queries
{
    /// <summary>
    /// Specialized query handler for audit data retrieval
    /// </summary>
    public class AuditQueryHandler
    {
        private readonly ApplicationDbContext _dbContext;

        public AuditQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Get audit trail with advanced filtering options
        /// </summary>
        public async Task<(IEnumerable<AuditRecord> Records, int TotalCount)> GetAuditTrailAsync(
            string tenantId,
            string entityName,
            string entityId,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string userId = null,
            string action = null,
            string searchTerm = null,
            int pageSize = 100,
            int pageNumber = 1)
        {
            // Build base query
            var query = _dbContext.Audits.AsQueryable();

            // Apply tenant filter
            if (!string.IsNullOrEmpty(tenantId))
            {
                query = query.Where(a => a.TenantId == tenantId);
            }

            // Apply entity filters
            if (!string.IsNullOrEmpty(entityName))
            {
                query = query.Where(a => a.EntityName == entityName);
            }

            if (!string.IsNullOrEmpty(entityId))
            {
                query = query.Where(a => a.EntityId == entityId);
            }

            // Apply date filters
            if (startDate.HasValue)
            {
                query = query.Where(a => a.Timestamp >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(a => a.Timestamp <= endDate.Value);
            }

            // Apply user filter
            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(a => a.UserId == userId);
            }

            // Apply action filter
            if (!string.IsNullOrEmpty(action))
            {
                query = query.Where(a => a.Action == action);
            }

            // Apply search term
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a =>
                    a.EntityName.Contains(searchTerm) ||
                    a.EntityId.Contains(searchTerm) ||
                    a.OriginalValues.Contains(searchTerm) ||
                    a.NewValues.Contains(searchTerm));
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply paging
            var skip = (pageNumber - 1) * pageSize;
            var records = await query
                .OrderByDescending(a => a.Timestamp)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            // Map to audit records
            var auditRecords = records.Select(a => new AuditRecord
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
                ProcessedAt = a.ProcessedAt,
                // Calculate changes for display
                Changes = CalculateChanges(a.OriginalValues, a.NewValues)
            });

            return (auditRecords, totalCount);
        }

        /// <summary>
        /// Calculate changes between original and new values
        /// </summary>
        private string CalculateChanges(string originalValues, string newValues)
        {
            if (string.IsNullOrEmpty(originalValues) || string.IsNullOrEmpty(newValues))
            {
                return null;
            }

            try
            {
                // Parse JSON values
                var original = System.Text.Json.JsonDocument.Parse(originalValues);
                var updated = System.Text.Json.JsonDocument.Parse(newValues);

                // Compare and build changes
                var changes = new Dictionary<string, (string Original, string New)>();

                // Process all properties in original
                foreach (var property in original.RootElement.EnumerateObject())
                {
                    if (updated.RootElement.TryGetProperty(property.Name, out var newValue))
                    {
                        var originalValue = property.Value.ToString();
                        var updatedValue = newValue.ToString();

                        if (originalValue != updatedValue)
                        {
                            changes[property.Name] = (originalValue, updatedValue);
                        }
                    }
                }

                // Process new properties not in original
                foreach (var property in updated.RootElement.EnumerateObject())
                {
                    if (!original.RootElement.TryGetProperty(property.Name, out _))
                    {
                        changes[property.Name] = (null, property.Value.ToString());
                    }
                }

                // Format changes as JSON
                return System.Text.Json.JsonSerializer.Serialize(changes);
            }
            catch
            {
                // If parsing fails, return null
                return null;
            }
        }

        /// <summary>
        /// Get audit statistics for a tenant
        /// </summary>
        public async Task<AuditStatistics> GetAuditStatisticsAsync(
            string tenantId,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            // Build base query
            var query = _dbContext.Audits.AsQueryable();

            // Apply tenant filter
            if (!string.IsNullOrEmpty(tenantId))
            {
                query = query.Where(a => a.TenantId == tenantId);
            }

            // Apply date filters
            if (startDate.HasValue)
            {
                query = query.Where(a => a.Timestamp >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(a => a.Timestamp <= endDate.Value);
            }

            // Get statistics
            var stats = new AuditStatistics
            {
                TotalRecords = await query.CountAsync(),
                CreateCount = await query.CountAsync(a => a.Action == "CREATE"),
                UpdateCount = await query.CountAsync(a => a.Action == "UPDATE"),
                DeleteCount = await query.CountAsync(a => a.Action == "DELETE"),
                EntityCounts = await query
                    .GroupBy(a => a.EntityName)
                    .Select(g => new EntityAuditCount
                    {
                        EntityName = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync(),
                UserCounts = await query
                    .GroupBy(a => a.UserId)
                    .Select(g => new UserAuditCount
                    {
                        UserId = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync()
            };

            return stats;
        }
    }

    /// <summary>
    /// Statistics about audit records
    /// </summary>
    public class AuditStatistics
    {
        public int TotalRecords { get; set; }
        public int CreateCount { get; set; }
        public int UpdateCount { get; set; }
        public int DeleteCount { get; set; }
        public List<EntityAuditCount> EntityCounts { get; set; }
        public List<UserAuditCount> UserCounts { get; set; }
    }

    /// <summary>
    /// Count of audit records per entity
    /// </summary>
    public class EntityAuditCount
    {
        public string EntityName { get; set; }
        public int Count { get; set; }
    }

    /// <summary>
    /// Count of audit records per user
    /// </summary>
    public class UserAuditCount
    {
        public string UserId { get; set; }
        public int Count { get; set; }
    }
}
