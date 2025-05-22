using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using SchoolManagement.Domain.Models.Audit;

namespace SchoolManagement.Infrastructure.Actors.Audit
{
    /// <summary>
    /// The root actor for the audit logging system. Coordinates all audit operations.
    /// </summary>
    public class AuditCoordinatorActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private readonly Dictionary<string, IActorRef> _tenantActors = new Dictionary<string, IActorRef>();
        private IActorRef _tenantManagerActor;
        private IActorRef _persistenceManagerActor;

        public AuditCoordinatorActor()
        {
            // Initialize child actors
            _tenantManagerActor = Context.ActorOf(Props.Create(() => new TenantManagerActor()), "tenant-manager");
            _persistenceManagerActor = Context.ActorOf(Props.Create(() => new PersistenceManagerActor()), "persistence-manager");

            // Define message handlers
            ReceiveAsync<CreateAuditCommand>(HandleCreateAuditCommand);
            ReceiveAsync<UpdateAuditCommand>(HandleUpdateAuditCommand);
            ReceiveAsync<DeleteAuditCommand>(HandleDeleteAuditCommand);
            Receive<TenantActorCreated>(msg => _tenantActors[msg.TenantId] = msg.TenantActor);
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                maxNrOfRetries: 10,
                withinTimeRange: TimeSpan.FromMinutes(1),
                localOnlyDecider: ex =>
                {
                    _logger.Error(ex, "Error in child actor");
                    
                    switch (ex)
                    {
                        case ActorInitializationException _:
                            return Directive.Restart;
                        case ActorKilledException _:
                            return Directive.Stop;
                        case Exception _:
                            return Directive.Restart;
                        default:
                            return Directive.Escalate;
                    }
                });
        }

        private async Task HandleCreateAuditCommand(CreateAuditCommand command)
        {
            try
            {
                _logger.Debug("Received create audit command for {EntityName} {EntityId} in tenant {TenantId}",
                    command.EntityName, command.EntityId, command.TenantId);
                
                // Forward to tenant manager
                var result = await _tenantManagerActor.Ask<bool>(command);
                
                // Reply to sender
                Sender.Tell(result);
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
                _logger.Debug("Received update audit command for {EntityName} {EntityId} in tenant {TenantId}",
                    command.EntityName, command.EntityId, command.TenantId);
                
                // Forward to tenant manager
                var result = await _tenantManagerActor.Ask<bool>(command);
                
                // Reply to sender
                Sender.Tell(result);
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
                _logger.Debug("Received delete audit command for {EntityName} {EntityId} in tenant {TenantId}",
                    command.EntityName, command.EntityId, command.TenantId);
                
                // Forward to tenant manager
                var result = await _tenantManagerActor.Ask<bool>(command);
                
                // Reply to sender
                Sender.Tell(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error handling delete audit command");
                throw;
            }
        }
    }

    /// <summary>
    /// Message sent when a new tenant actor is created
    /// </summary>
    public class TenantActorCreated
    {
        public string TenantId { get; }
        public IActorRef TenantActor { get; }

        public TenantActorCreated(string tenantId, IActorRef tenantActor)
        {
            TenantId = tenantId;
            TenantActor = tenantActor;
        }
    }
}
