namespace EHS.Domain.Entities
{
    /// <summary>
    /// Stores the before/after values for each property changed in an audit log entry.
    /// Provides detailed tracking of what changed and why.
    /// </summary>
    public class AuditLogDetail
    {
        /// <summary>
        /// Unique identifier for the detail entry.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Name of the property that was changed.
        /// </summary>
        public string PropertyName { get; set; } = string.Empty;

        /// <summary>
        /// Previous value of the property (before the change).
        /// Null for new records.
        /// </summary>
        public string? OldValue { get; set; }

        /// <summary>
        /// New value of the property (after the change).
        /// </summary>
        public string? NewValue { get; set; }

        /// <summary>
        /// Data type of the property for proper interpretation.
        /// </summary>
        public string PropertyType { get; set; } = string.Empty;

        /// <summary>
        /// Foreign key for the parent audit log.
        /// </summary>
        public Guid AuditLogId { get; set; }

        /// <summary>
        /// Navigation property for the parent audit log.
        /// </summary>
        public AuditLog AuditLog { get; set; } = null!;
    }
}