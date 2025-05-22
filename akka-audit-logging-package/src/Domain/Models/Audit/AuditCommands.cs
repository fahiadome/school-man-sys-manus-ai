using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using SchoolManagement.Domain.Models.Audit;

namespace SchoolManagement.Infrastructure.Actors.Audit
{
    /// <summary>
    /// Base class for audit commands
    /// </summary>
    public abstract class AuditCommand
    {
        public string TenantId { get; set; }
        public string EntityName { get; set; }
        public string EntityId { get; set; }
        public string UserId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Command to create an audit record for entity creation
    /// </summary>
    public class CreateAuditCommand : AuditCommand
    {
        public string NewValues { get; set; }
    }

    /// <summary>
    /// Command to create an audit record for entity update
    /// </summary>
    public class UpdateAuditCommand : AuditCommand
    {
        public string OriginalValues { get; set; }
        public string NewValues { get; set; }
    }

    /// <summary>
    /// Command to create an audit record for entity deletion
    /// </summary>
    public class DeleteAuditCommand : AuditCommand
    {
        public string OriginalValues { get; set; }
    }
}
