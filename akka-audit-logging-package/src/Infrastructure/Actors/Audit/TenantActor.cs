using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;

namespace SchoolManagement.Infrastructure.Actors.Audit
{
    /// <summary>
    /// Actor responsible for handling audit events for a specific tenant
    /// </summary>
    public class TenantActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private readonly string _tenantId;
        private readonly Dictionary<string, IActorRef> _entityTypeActors = new Dictionary<string, IActorRef>();
        private readonly ThrottlingSettings _throttlingSettings;

        public TenantActor(string tenantId)
        {
            _tenantId = tenantId;
            _throttlingSettings = new ThrottlingSettings(
                maxEventsPerSecond: 1000, // Configurable
                burstSize: 2000 // Configurable
            );

            // Define message handlers
            ReceiveAsync<CreateAuditCommand>(HandleCreateAuditCommand);
            ReceiveAsync<UpdateAuditCommand>(HandleUpdateAuditCommand);
            ReceiveAsync<DeleteAuditCommand>(HandleDeleteAuditCommand);
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                maxNrOfRetries: 3,
                withinTimeRange: TimeSpan.FromSeconds(10),
                localOnlyDecider: ex =>
                {
                    _logger.Error(ex, "Error in entity type actor for tenant {TenantId}", _tenantId);
                    
                    switch (ex)
                    {
                        case Exception _:
                            return Directive.Restart;
                        default:
                            return Directive.Escalate;
                    }
                });
        }

        private async Task HandleCreateAuditCommand(CreateAuditCommand command)
        {
            if (command.TenantId != _tenantId)
            {
                _logger.Warning("Received command for wrong tenant. Expected {ExpectedTenantId}, got {ActualTenantId}",
                    _tenantId, command.TenantId);
                throw new ArgumentException($"Command for wrong tenant: {command.TenantId}");
            }

            if (_throttlingSettings.ShouldThrottle())
            {
                _logger.Warning("Throttling create audit command for tenant {TenantId}", _tenantId);
                throw new ThrottlingException($"Tenant {_tenantId} is being throttled");
            }

            var entityTypeActor = GetOrCreateEntityTypeActor(command.EntityName);
            var result = await entityTypeActor.Ask<bool>(command);
            
            Sender.Tell(result);
        }

        private async Task HandleUpdateAuditCommand(UpdateAuditCommand command)
        {
            if (command.TenantId != _tenantId)
            {
                _logger.Warning("Received command for wrong tenant. Expected {ExpectedTenantId}, got {ActualTenantId}",
                    _tenantId, command.TenantId);
                throw new ArgumentException($"Command for wrong tenant: {command.TenantId}");
            }

            if (_throttlingSettings.ShouldThrottle())
            {
                _logger.Warning("Throttling update audit command for tenant {TenantId}", _tenantId);
                throw new ThrottlingException($"Tenant {_tenantId} is being throttled");
            }

            var entityTypeActor = GetOrCreateEntityTypeActor(command.EntityName);
            var result = await entityTypeActor.Ask<bool>(command);
            
            Sender.Tell(result);
        }

        private async Task HandleDeleteAuditCommand(DeleteAuditCommand command)
        {
            if (command.TenantId != _tenantId)
            {
                _logger.Warning("Received command for wrong tenant. Expected {ExpectedTenantId}, got {ActualTenantId}",
                    _tenantId, command.TenantId);
                throw new ArgumentException($"Command for wrong tenant: {command.TenantId}");
            }

            if (_throttlingSettings.ShouldThrottle())
            {
                _logger.Warning("Throttling delete audit command for tenant {TenantId}", _tenantId);
                throw new ThrottlingException($"Tenant {_tenantId} is being throttled");
            }

            var entityTypeActor = GetOrCreateEntityTypeActor(command.EntityName);
            var result = await entityTypeActor.Ask<bool>(command);
            
            Sender.Tell(result);
        }

        private IActorRef GetOrCreateEntityTypeActor(string entityName)
        {
            if (!_entityTypeActors.TryGetValue(entityName, out var entityTypeActor))
            {
                _logger.Debug("Creating new entity type actor for {EntityName} in tenant {TenantId}", 
                    entityName, _tenantId);
                
                // Create new entity type actor
                entityTypeActor = Context.ActorOf(
                    Props.Create(() => new EntityTypeActor(_tenantId, entityName)), 
                    $"entity-{entityName.ToLower()}");
                
                _entityTypeActors[entityName] = entityTypeActor;
            }
            
            return entityTypeActor;
        }
    }

    /// <summary>
    /// Settings for throttling audit events
    /// </summary>
    public class ThrottlingSettings
    {
        private readonly int _maxEventsPerSecond;
        private readonly int _burstSize;
        private readonly Queue<DateTime> _eventTimestamps = new Queue<DateTime>();
        private readonly object _lock = new object();

        public ThrottlingSettings(int maxEventsPerSecond, int burstSize)
        {
            _maxEventsPerSecond = maxEventsPerSecond;
            _burstSize = burstSize;
        }

        public bool ShouldThrottle()
        {
            lock (_lock)
            {
                var now = DateTime.UtcNow;
                
                // Remove timestamps older than 1 second
                while (_eventTimestamps.Count > 0 && (now - _eventTimestamps.Peek()).TotalSeconds > 1)
                {
                    _eventTimestamps.Dequeue();
                }
                
                // Check if we're over the limit
                if (_eventTimestamps.Count >= _maxEventsPerSecond)
                {
                    // Allow burst up to burst size
                    if (_eventTimestamps.Count >= _burstSize)
                    {
                        return true; // Throttle
                    }
                }
                
                // Add current timestamp
                _eventTimestamps.Enqueue(now);
                
                return false; // Don't throttle
            }
        }
    }

    /// <summary>
    /// Exception thrown when throttling is applied
    /// </summary>
    public class ThrottlingException : Exception
    {
        public ThrottlingException(string message) : base(message)
        {
        }
    }
}
