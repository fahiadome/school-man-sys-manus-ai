using System;

namespace SchoolManagement.Domain.Entities.Audit
{
    /// <summary>
    /// Represents an audit record in the system
    /// </summary>
    public class Audit
    {
        /// <summary>
        /// Unique identifier for the audit record
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// Tenant identifier (null for system-level operations)
        /// </summary>
        public string TenantId { get; set; }
        
        /// <summary>
        /// Name of the entity being audited (e.g., "Student", "Grade", "User")
        /// </summary>
        public string EntityName { get; set; }
        
        /// <summary>
        /// Primary key of the affected entity
        /// </summary>
        public string EntityId { get; set; }
        
        /// <summary>
        /// Action performed ("CREATE", "UPDATE", "DELETE")
        /// </summary>
        public string Action { get; set; }
        
        /// <summary>
        /// User who performed the action
        /// </summary>
        public string UserId { get; set; }
        
        /// <summary>
        /// When the action occurred
        /// </summary>
        public DateTime Timestamp { get; set; }
        
        /// <summary>
        /// JSON representation of original values (for UPDATE/DELETE)
        /// </summary>
        public string OriginalValues { get; set; }
        
        /// <summary>
        /// JSON representation of new values (for CREATE/UPDATE)
        /// </summary>
        public string NewValues { get; set; }
        
        /// <summary>
        /// When the audit was processed
        /// </summary>
        public DateTime ProcessedAt { get; set; }
    }
}
