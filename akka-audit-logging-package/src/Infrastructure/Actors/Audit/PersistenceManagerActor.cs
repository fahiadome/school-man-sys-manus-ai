using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Akka.Persistence;
using SchoolManagement.Domain.Entities.Audit;

namespace SchoolManagement.Infrastructure.Actors.Audit
{
    /// <summary>
    /// Actor responsible for managing persistence of audit events
    /// </summary>
    public class PersistenceManagerActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private readonly Dictionary<string, IActorRef> _batchProcessorActors = new Dictionary<string, IActorRef>();

        public PersistenceManagerActor()
        {
            // Define message handlers
            Receive<BatchAuditEvents>(HandleBatchAuditEvents);
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                maxNrOfRetries: 5,
                withinTimeRange: TimeSpan.FromMinutes(1),
                localOnlyDecider: ex =>
                {
                    _logger.Error(ex, "Error in batch processor actor");
                    
                    switch (ex)
                    {
                        case System.Data.Common.DbException _:
                            return Directive.Restart;
                        case TimeoutException _:
                            return Directive.Restart;
                        default:
                            return Directive.Escalate;
                    }
                });
        }

        private void HandleBatchAuditEvents(BatchAuditEvents batch)
        {
            try
            {
                _logger.Debug("Received batch of {Count} audit events for tenant {TenantId}", 
                    batch.Events.Count(), batch.TenantId);
                
                // Get or create batch processor actor for this tenant
                var batchProcessorActor = GetOrCreateBatchProcessorActor(batch.TenantId);
                
                // Forward batch to processor
                batchProcessorActor.Forward(batch);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error handling batch audit events");
                throw;
            }
        }

        private IActorRef GetOrCreateBatchProcessorActor(string tenantId)
        {
            if (!_batchProcessorActors.TryGetValue(tenantId, out var batchProcessorActor))
            {
                _logger.Debug("Creating new batch processor actor for tenant {TenantId}", tenantId);
                
                // Create new batch processor actor
                batchProcessorActor = Context.ActorOf(
                    Props.Create(() => new BatchProcessorActor(tenantId)), 
                    $"batch-processor-{tenantId}");
                
                _batchProcessorActors[tenantId] = batchProcessorActor;
            }
            
            return batchProcessorActor;
        }
    }

    /// <summary>
    /// Actor responsible for processing batches of audit events for a specific tenant
    /// </summary>
    public class BatchProcessorActor : PersistentActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private readonly string _tenantId;
        private readonly IAuditRepository _auditRepository;
        private int _processedBatches = 0;

        public BatchProcessorActor(string tenantId)
        {
            _tenantId = tenantId;
            
            // In a real implementation, this would be injected via dependency injection
            // For this example, we're creating it directly
            _auditRepository = new AuditRepository();
        }

        public override string PersistenceId => $"batch-processor-{_tenantId}";

        protected override void OnRecover(object message)
        {
            switch (message)
            {
                case BatchProcessed batchProcessed:
                    _processedBatches++;
                    break;
                
                case RecoveryCompleted _:
                    _logger.Info("Recovery completed for batch processor {PersistenceId}, processed batches: {ProcessedBatches}", 
                        PersistenceId, _processedBatches);
                    break;
            }
        }

        protected override void OnCommand(object message)
        {
            switch (message)
            {
                case BatchAuditEvents batch:
                    HandleBatchAuditEvents(batch);
                    break;
                
                case BatchProcessed batchProcessed:
                    // Confirmation that batch was processed
                    _processedBatches++;
                    break;
            }
        }

        private void HandleBatchAuditEvents(BatchAuditEvents batch)
        {
            try
            {
                _logger.Debug("Processing batch of {Count} audit events for tenant {TenantId}", 
                    batch.Events.Count(), batch.TenantId);
                
                // Convert to audit entities
                var auditEntities = batch.Events.Select(e => new Audit
                {
                    TenantId = e.TenantId,
                    EntityName = e.EntityName,
                    EntityId = e.EntityId,
                    Action = e.Action,
                    UserId = e.UserId,
                    Timestamp = e.Timestamp,
                    OriginalValues = e.OriginalValues,
                    NewValues = e.NewValues,
                    ProcessedAt = DateTime.UtcNow
                }).ToList();
                
                // Persist event before saving to database
                Persist(new BatchProcessed(batch.TenantId, auditEntities.Count), persistedEvent =>
                {
                    // Save to database
                    _auditRepository.SaveAuditBatchAsync(auditEntities).Wait();
                    
                    // Update state
                    Self.Tell(persistedEvent);
                    
                    // Confirm to sender
                    Sender.Tell(new BatchSaved(batch.TenantId, auditEntities.Count));
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error processing batch audit events");
                throw;
            }
        }
    }

    /// <summary>
    /// Event indicating a batch was processed
    /// </summary>
    public class BatchProcessed
    {
        public string TenantId { get; }
        public int Count { get; }

        public BatchProcessed(string tenantId, int count)
        {
            TenantId = tenantId;
            Count = count;
        }
    }

    /// <summary>
    /// Message indicating a batch was saved to the database
    /// </summary>
    public class BatchSaved
    {
        public string TenantId { get; }
        public int Count { get; }

        public BatchSaved(string tenantId, int count)
        {
            TenantId = tenantId;
            Count = count;
        }
    }
}
