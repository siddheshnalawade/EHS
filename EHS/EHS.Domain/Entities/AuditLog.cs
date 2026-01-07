namespace EHS.Domain.Entities
{
    /// <summary>
    /// Records all significant changes to entities in the system.
    /// Provides a complete audit trail for compliance and analysis.
    /// </summary>
    public class AuditLog
    {
        /// <summary>
        /// Unique identifier for the audit log entry.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Fully qualified entity type name (e.g., "EHS.Domain.Entities.Incident").
        /// </summary>
        public string EntityType { get; set; } = string.Empty;

        /// <summary>
        /// ID of the entity that was modified.
        /// </summary>
        public string EntityId { get; set; } = string.Empty;

        /// <summary>
        /// Type of change: "Create", "Update", "Delete", "Reassign", "StatusChange".
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// User ID of who made the change.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Timestamp when the change was made.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// IP address from which the change was made.
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// User agent of the browser/client that made the change.
        /// </summary>
        public string? UserAgent { get; set; }

        /// <summary>
        /// Additional context or reason for the change.
        /// </summary>
        public string? Remarks { get; set; }

        /// <summary>
        /// Navigation property for detailed property changes.
        /// </summary>
        public List<AuditLogDetail> Details { get; set; } = [];

        /// <summary>
        /// Optional reference to the incident if this audit is incident-related.
        /// </summary>
        public Guid? IncidentId { get; set; }

        /// <summary>
        /// Navigation property for the incident.
        /// </summary>
        public Incident? Incident { get; set; }
    }
}