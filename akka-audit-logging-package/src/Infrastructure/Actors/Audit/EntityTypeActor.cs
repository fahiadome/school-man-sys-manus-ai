using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;

namespace SchoolManagement.Infrastructure.Actors.Audit
{
    /// <summary>
    /// Actor responsible for processing audit events for a specific entity type within a tenant
    /// </summary>
    public class EntityTypeActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private readonly string _tenantId;
        private readonly string _entityName;
        private readonly IActorRef _persistenceManagerActor;
        private readonly List<AuditEvent> _pendingEvents = new List<AuditEvent>();
        private readonly int _batchSize = 10; // Configurable
        private ICancelable _batchScheduler;

        public EntityTypeActor(string tenantId, string entityName)
        {
            _tenantId = tenantId;
            _entityName = entityName;
            
            // Get reference to persistence manager actor
            _persistenceManagerActor = Context.ActorSelection("/user/audit-coordinator/persistence-manager")
                .ResolveOne(TimeSpan.FromSeconds(5))
                .GetAwaiter()
                .GetResult();

            // Define message handlers
            ReceiveAsync<CreateAuditCommand>(HandleCreateAuditCommand);
            ReceiveAsync<UpdateAuditCommand>(HandleUpdateAuditCommand);
            ReceiveAsync<DeleteAuditCommand>(HandleDeleteAuditCommand);
            Receive<FlushPendingEvents>(HandleFlushPendingEvents);
        }

        protected override void PreStart()
        {
            base.PreStart();
            
            // Schedule periodic batch processing
            _batchScheduler = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
                TimeSpan.FromSeconds(2), // Initial delay
                TimeSpan.FromSeconds(2), // Interval
                Self,
                new FlushPendingEvents(),
                Self);
        }

        protected override void PostStop()
        {
            // Cancel scheduler
            _batchScheduler?.Cancel();
            
            // Flush any pending events
            if (_pendingEvents.Count > 0)
            {
                FlushEvents();
            }
            
            base.PostStop();
        }

        private async Task HandleCreateAuditCommand(CreateAuditCommand command)
        {
            try
            {
                _logger.Debug("Processing create audit command for {EntityName} {EntityId} in tenant {TenantId}",
                    command.EntityName, command.EntityId, command.TenantId);
                
                // Create audit event
                var auditEvent = new AuditEvent
                {
                    TenantId = command.TenantId,
                    EntityName = command.EntityName,
                    EntityId = command.EntityId,
                    Action = "CREATE",
                    UserId = command.UserId,
                    Timestamp = command.Timestamp,
                    NewValues = command.NewValues,
                    OriginalValues = null
                };
                
                // Add to pending events
                _pendingEvents.Add(auditEvent);
                
                // If batch size reached, flush events
                if (_pendingEvents.Count >= _batchSize)
                {
                    FlushEvents();
                }
                
                Sender.Tell(true);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error handling create audit command");
                throw;
            }
        }

        private async Task HandleUpdateAuditCommand(UpdateAuditCommand command)
        {
            try
            {
                _logger.Debug("Processing update audit command for {EntityName} {EntityId} in tenant {TenantId}",
                    command.EntityName, command.EntityId, command.TenantId);
                
                // Create audit event
                var auditEvent = new AuditEvent
                {
                    TenantId = command.TenantId,
                    EntityName = command.EntityName,
                    EntityId = command.EntityId,
                    Action = "UPDATE",
                    UserId = command.UserId,
                    Timestamp = command.Timestamp,
                    NewValues = command.NewValues,
                    OriginalValues = command.OriginalValues
                };
                
                // Add to pending events
                _pendingEvents.Add(auditEvent);
                
                // If batch size reached, flush events
                if (_pendingEvents.Count >= _batchSize)
                {
                    FlushEvents();
                }
                
                Sender.Tell(true);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error handling update audit command");
                throw;
            }
        }

        private async Task HandleDeleteAuditCommand(DeleteAuditCommand command)
        {
            try
            {
                _logger.Debug("Processing delete audit command for {EntityName} {EntityId} in tenant {TenantId}",
                    command.EntityName, command.EntityId, command.TenantId);
                
                // Create audit event
                var auditEvent = new AuditEvent
                {
                    TenantId = command.TenantId,
                    EntityName = command.EntityName,
                    EntityId = command.EntityId,
                    Action = "DELETE",
                    UserId = command.UserId,
                    Timestamp = command.Timestamp,
                    NewValues = null,
                    OriginalValues = command.OriginalValues
                };
                
                // Add to pending events
                _pendingEvents.Add(auditEvent);
                
                // If batch size reached, flush events
                if (_pendingEvents.Count >= _batchSize)
                {
                    FlushEvents();
                }
                
                Sender.Tell(true);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error handling delete audit command");
                throw;
            }
        }

        private void HandleFlushPendingEvents(FlushPendingEvents message)
        {
            if (_pendingEvents.Count > 0)
            {
                FlushEvents();
            }
        }

        private void FlushEvents()
        {
            try
            {
                _logger.Debug("Flushing {Count} pending events for {EntityName} in tenant {TenantId}",
                    _pendingEvents.Count, _entityName, _tenantId);
                
                // Create batch message
                var batchMessage = new BatchAuditEvents
                {
                    TenantId = _tenantId,
                    Events = new List<AuditEvent>(_pendingEvents)
                };
                
                // Send to persistence manager
                _persistenceManagerActor.Tell(batchMessage);
                
                // Clear pending events
                _pendingEvents.Clear();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error flushing events");
                throw;
            }
        }
    }

    /// <summary>
    /// Message to trigger flushing of pending events
    /// </summary>
    public class FlushPendingEvents
    {
    }

    /// <summary>
    /// Audit event data
    /// </summary>
    public class AuditEvent
    {
        public string TenantId { get; set; }
        public string EntityName { get; set; }
        public string EntityId { get; set; }
        public string Action { get; set; }
        public string UserId { get; set; }
        public DateTime Timestamp { get; set; }
        public string OriginalValues { get; set; }
        public string NewValues { get; set; }
    }

    /// <summary>
    /// Message containing a batch of audit events
    /// </summary>
    public class BatchAuditEvents
    {
        public string TenantId { get; set; }
        public IEnumerable<AuditEvent> Events { get; set; }
    }
}
