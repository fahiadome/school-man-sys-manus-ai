using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.Extensions.Logging;
using SchoolManagement.Domain.Interfaces.Services;
using SchoolManagement.Domain.Models.Audit;
using SchoolManagement.Infrastructure.Actors;
using System.Text.Json;

namespace SchoolManagement.Application.Services
{
    /// <summary>
    /// Implementation of the audit service using Akka.NET actors
    /// </summary>
    public class AkkaAuditService : IAuditService
    {
        private readonly ILogger<AkkaAuditService> _logger;
        private readonly ActorSystemFactory _actorSystemFactory;
        private readonly IActorRef _auditCoordinatorActor;
        private readonly IActorRef _auditQueryActor;

        public AkkaAuditService(
            ILogger<AkkaAuditService> logger,
            ActorSystemFactory actorSystemFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _actorSystemFactory = actorSystemFactory ?? throw new ArgumentNullException(nameof(actorSystemFactory));
            
            // Get actor references
            _auditCoordinatorActor = _actorSystemFactory.GetAuditCoordinatorActor();
            _auditQueryActor = _actorSystemFactory.GetAuditQueryActor();
        }

        /// <summary>
        /// Log an entity creation event
        /// </summary>
        public async Task LogCreateAsync<T>(string tenantId, string entityId, T entity, string userId)
        {
            try
            {
                _logger.LogDebug("Logging create event for {EntityType} {EntityId} in tenant {TenantId}",
                    typeof(T).Name, entityId, tenantId);
                
                // Create command
                var command = new CreateAuditCommand
                {
                    TenantId = tenantId,
                    EntityName = typeof(T).Name,
                    EntityId = entityId,
                    UserId = userId,
                    Timestamp = DateTime.UtcNow,
                    NewValues = JsonSerializer.Serialize(entity)
                };
                
                // Send to actor
                var result = await _auditCoordinatorActor.Ask<bool>(command);
                
                if (!result)
                {
                    _logger.LogWarning("Failed to log create event for {EntityType} {EntityId} in tenant {TenantId}",
                        typeof(T).Name, entityId, tenantId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging create event for {EntityType} {EntityId} in tenant {TenantId}",
                    typeof(T).Name, entityId, tenantId);
                
                // Don't rethrow - audit logging should not affect main operations
            }
        }

        /// <summary>
        /// Log an entity update event
        /// </summary>
        public async Task LogUpdateAsync<T>(string tenantId, string entityId, T originalEntity, T updatedEntity, string userId)
        {
            try
            {
                _logger.LogDebug("Logging update event for {EntityType} {EntityId} in tenant {TenantId}",
                    typeof(T).Name, entityId, tenantId);
                
                // Create command
                var command = new UpdateAuditCommand
                {
                    TenantId = tenantId,
                    EntityName = typeof(T).Name,
                    EntityId = entityId,
                    UserId = userId,
                    Timestamp = DateTime.UtcNow,
                    OriginalValues = JsonSerializer.Serialize(originalEntity),
                    NewValues = JsonSerializer.Serialize(updatedEntity)
                };
                
                // Send to actor
                var result = await _auditCoordinatorActor.Ask<bool>(command);
                
                if (!result)
                {
                    _logger.LogWarning("Failed to log update event for {EntityType} {EntityId} in tenant {TenantId}",
                        typeof(T).Name, entityId, tenantId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging update event for {EntityType} {EntityId} in tenant {TenantId}",
                    typeof(T).Name, entityId, tenantId);
                
                // Don't rethrow - audit logging should not affect main operations
            }
        }

        /// <summary>
        /// Log an entity deletion event
        /// </summary>
        public async Task LogDeleteAsync<T>(string tenantId, string entityId, T entity, string userId)
        {
            try
            {
                _logger.LogDebug("Logging delete event for {EntityType} {EntityId} in tenant {TenantId}",
                    typeof(T).Name, entityId, tenantId);
                
                // Create command
                var command = new DeleteAuditCommand
                {
                    TenantId = tenantId,
                    EntityName = typeof(T).Name,
                    EntityId = entityId,
                    UserId = userId,
                    Timestamp = DateTime.UtcNow,
                    OriginalValues = JsonSerializer.Serialize(entity)
                };
                
                // Send to actor
                var result = await _auditCoordinatorActor.Ask<bool>(command);
                
                if (!result)
                {
                    _logger.LogWarning("Failed to log delete event for {EntityType} {EntityId} in tenant {TenantId}",
                        typeof(T).Name, entityId, tenantId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging delete event for {EntityType} {EntityId} in tenant {TenantId}",
                    typeof(T).Name, entityId, tenantId);
                
                // Don't rethrow - audit logging should not affect main operations
            }
        }

        /// <summary>
        /// Get audit trail for an entity
        /// </summary>
        public async Task<IEnumerable<AuditRecord>> GetAuditTrailAsync(
            string tenantId, 
            string entityName, 
            string entityId, 
            DateTime? startDate = null, 
            DateTime? endDate = null, 
            int pageSize = 100, 
            int pageNumber = 1)
        {
            try
            {
                _logger.LogDebug("Getting audit trail for {EntityName} {EntityId} in tenant {TenantId}",
                    entityName, entityId, tenantId);
                
                // Create query
                var query = new GetAuditTrailQuery
                {
                    TenantId = tenantId,
                    EntityName = entityName,
                    EntityId = entityId,
                    StartDate = startDate,
                    EndDate = endDate,
                    PageSize = pageSize,
                    PageNumber = pageNumber
                };
                
                // Send to actor
                var result = await _auditQueryActor.Ask<AuditTrailResult>(query);
                
                return result.Records;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting audit trail for {EntityName} {EntityId} in tenant {TenantId}",
                    entityName, entityId, tenantId);
                
                throw;
            }
        }
    }
}
