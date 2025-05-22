using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;

namespace SchoolManagement.Infrastructure.Actors.Audit
{
    /// <summary>
    /// Actor for handling audit queries
    /// </summary>
    public class AuditQueryActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private readonly Dictionary<string, IActorRef> _tenantQueryActors = new Dictionary<string, IActorRef>();
        
        public AuditQueryActor()
        {
            ReceiveAsync<GetAuditTrailQuery>(async query =>
            {
                try
                {
                    _logger.Debug("Received audit trail query for {EntityName} {EntityId} in tenant {TenantId}",
                        query.EntityName, query.EntityId, query.TenantId);
                    
                    var tenantQueryActor = GetOrCreateTenantQueryActor(query.TenantId);
                    var result = await tenantQueryActor.Ask<AuditTrailResult>(query);
                    
                    Sender.Tell(result);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error handling audit trail query");
                    throw;
                }
            });
        }
        
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                maxNrOfRetries: 3,
                withinTimeRange: TimeSpan.FromSeconds(30),
                localOnlyDecider: ex =>
                {
                    _logger.Error(ex, "Error in tenant query actor");
                    
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
        
        private IActorRef GetOrCreateTenantQueryActor(string tenantId)
        {
            if (!_tenantQueryActors.TryGetValue(tenantId, out var tenantQueryActor))
            {
                _logger.Debug("Creating new tenant query actor for tenant {TenantId}", tenantId);
                
                tenantQueryActor = Context.ActorOf(
                    Props.Create(() => new TenantQueryActor(tenantId)), 
                    $"tenant-query-{tenantId}");
                    
                _tenantQueryActors[tenantId] = tenantQueryActor;
            }
            
            return tenantQueryActor;
        }
    }
    
    /// <summary>
    /// Actor for handling tenant-specific audit queries
    /// </summary>
    public class TenantQueryActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private readonly string _tenantId;
        private readonly IAuditRepository _auditRepository;
        
        public TenantQueryActor(string tenantId)
        {
            _tenantId = tenantId;
            _auditRepository = new AuditRepository();
            
            ReceiveAsync<GetAuditTrailQuery>(async query =>
            {
                try
                {
                    _logger.Debug("Processing audit trail query for {EntityName} {EntityId} in tenant {TenantId}",
                        query.EntityName, query.EntityId, query.TenantId);
                    
                    if (query.TenantId != _tenantId)
                    {
                        throw new ArgumentException($"Query for wrong tenant: {query.TenantId}");
                    }
                    
                    var records = await _auditRepository.GetAuditTrailAsync(
                        query.EntityName, 
                        query.EntityId, 
                        query.TenantId,
                        query.StartDate,
                        query.EndDate,
                        query.PageSize,
                        query.PageNumber);
                        
                    var totalCount = await _auditRepository.GetAuditTrailCountAsync(
                        query.EntityName, 
                        query.EntityId, 
                        query.TenantId,
                        query.StartDate,
                        query.EndDate);
                        
                    Sender.Tell(new AuditTrailResult
                    {
                        Records = records,
                        TotalCount = totalCount
                    });
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error processing audit trail query");
                    throw;
                }
            });
        }
    }
    
    /// <summary>
    /// Query to get audit trail for an entity
    /// </summary>
    public class GetAuditTrailQuery
    {
        public string TenantId { get; set; }
        public string EntityName { get; set; }
        public string EntityId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageSize { get; set; } = 100;
        public int PageNumber { get; set; } = 1;
    }
    
    /// <summary>
    /// Result of audit trail query
    /// </summary>
    public class AuditTrailResult
    {
        public IEnumerable<AuditRecord> Records { get; set; }
        public int TotalCount { get; set; }
    }
    
    /// <summary>
    /// Audit record returned from queries
    /// </summary>
    public class AuditRecord
    {
        public long Id { get; set; }
        public string TenantId { get; set; }
        public string EntityName { get; set; }
        public string EntityId { get; set; }
        public string Action { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime Timestamp { get; set; }
        public string OriginalValues { get; set; }
        public string NewValues { get; set; }
        public string Changes { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
