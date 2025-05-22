using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;

namespace SchoolManagement.Infrastructure.Actors.Audit
{
    /// <summary>
    /// Actor responsible for managing tenant-specific audit actors
    /// </summary>
    public class TenantManagerActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private readonly Dictionary<string, IActorRef> _tenantActors = new Dictionary<string, IActorRef>();

        public TenantManagerActor()
        {
            // Define message handlers
            ReceiveAsync<CreateAuditCommand>(HandleCreateAuditCommand);
            ReceiveAsync<UpdateAuditCommand>(HandleUpdateAuditCommand);
            ReceiveAsync<DeleteAuditCommand>(HandleDeleteAuditCommand);
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                maxNrOfRetries: 5,
                withinTimeRange: TimeSpan.FromSeconds(30),
                localOnlyDecider: ex =>
                {
                    _logger.Error(ex, "Error in tenant actor");
                    
                    switch (ex)
                    {
                        case System.Data.Common.DbException _:
                            return Directive.Restart;
                        case Exception _:
                            return Directive.Resume;
                        default:
                            return Directive.Escalate;
                    }
                });
        }

        private async Task HandleCreateAuditCommand(CreateAuditCommand command)
        {
            var tenantActor = GetOrCreateTenantActor(command.TenantId);
            var result = await tenantActor.Ask<bool>(command);
            Sender.Tell(result);
        }

        private async Task HandleUpdateAuditCommand(UpdateAuditCommand command)
        {
            var tenantActor = GetOrCreateTenantActor(command.TenantId);
            var result = await tenantActor.Ask<bool>(command);
            Sender.Tell(result);
        }

        private async Task HandleDeleteAuditCommand(DeleteAuditCommand command)
        {
            var tenantActor = GetOrCreateTenantActor(command.TenantId);
            var result = await tenantActor.Ask<bool>(command);
            Sender.Tell(result);
        }

        private IActorRef GetOrCreateTenantActor(string tenantId)
        {
            if (!_tenantActors.TryGetValue(tenantId, out var tenantActor))
            {
                _logger.Debug("Creating new tenant actor for tenant {TenantId}", tenantId);
                
                // Create new tenant actor
                tenantActor = Context.ActorOf(
                    Props.Create(() => new TenantActor(tenantId)), 
                    $"tenant-{tenantId}");
                
                _tenantActors[tenantId] = tenantActor;
                
                // Notify coordinator
                Context.Parent.Tell(new TenantActorCreated(tenantId, tenantActor));
            }
            
            return tenantActor;
        }
    }
}
