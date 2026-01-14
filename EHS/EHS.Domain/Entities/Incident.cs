namespace EHS.Domain.Entities
{
    /// <summary>
    /// Represents an incident report in the EHS system.
    /// This is the core entity for incident tracking and management.
    /// </summary>
    public class Incident : BaseEntity
    {
        /// <summary>
        /// Unique identifier for the incident.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key for the incident type (Near Miss, Unsafe Condition, Unsafe Action).
        /// </summary>
        public Guid IncidentTypeId { get; set; }

        /// <summary>
        /// Navigation property for the incident type.
        /// </summary>
        public IncidentType IncidentType { get; set; } = null!;

        /// <summary>
        /// Foreign key for the incident nature (New or Repeated).
        /// </summary>
        public Guid IncidentNatureId { get; set; }

        /// <summary>
        /// Navigation property for the incident nature.
        /// </summary>
        public IncidentNature IncidentNature { get; set; } = null!;

        /// <summary>
        /// Foreign key for the incident severity (Minor, Major, Catastrophic).
        /// </summary>
        public Guid IncidentSeverityId { get; set; }

        /// <summary>
        /// Navigation property for the incident severity.
        /// </summary>
        public IncidentSeverity IncidentSeverity { get; set; } = null!;

        /// <summary>
        /// Foreign key for the current incident status in the workflow.
        /// </summary>
        public Guid IncidentStatusId { get; set; }

        /// <summary>
        /// Navigation property for the incident status.
        /// </summary>
        public IncidentStatus IncidentStatus { get; set; } = null!;

        /// <summary>
        /// Date and time when the incident occurred.
        /// </summary>
        public DateTime IncidentDate { get; set; }

        /// <summary>
        /// Specific time the incident occurred (stored separately for querying).
        /// </summary>
        public TimeSpan? IncidentTime { get; set; }

        /// <summary>
        /// Foreign key for the organization where the incident occurred.
        /// </summary>
        public Guid OrganizationId { get; set; }

        /// <summary>
        /// Navigation property for the organization.
        /// </summary>
        public Organization Organization { get; set; } = null!;

        /// <summary>
        /// Foreign key for the department where the incident occurred.
        /// </summary>
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// Navigation property for the department.
        /// </summary>
        public Department Department { get; set; } = null!;

        /// <summary>
        /// Foreign key for the production line where the incident occurred.
        /// Optional as incidents may not always be line-specific.
        /// </summary>
        public Guid? ProductionLineId { get; set; }

        /// <summary>
        /// Navigation property for the production line.
        /// </summary>
        public ProductionLine? ProductionLine { get; set; }

        /// <summary>
        /// Foreign key for the machine involved in the incident.
        /// Optional as incidents may not always involve specific machines.
        /// </summary>
        public Guid? MachineId { get; set; }

        /// <summary>
        /// Navigation property for the machine.
        /// </summary>
        public Machine? Machine { get; set; }

        /// <summary>
        /// Short title summarizing the incident.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of the incident.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of the incident area.
        /// </summary>
        public string IncidentArea { get; set; } = string.Empty;

        /// <summary>
        /// Proposed solution by the incident initiator.
        /// </summary>
        public string ProposedSolution { get; set; } = string.Empty;

        /// <summary>
        /// User ID of the person who initiated the incident report.
        /// </summary>
        public Guid InitiatedByUserId { get; set; }

        /// <summary>
        /// Navigation property for the user who initiated the incident.
        /// </summary>
        public ApplicationUser InitiatedByUser { get; set; } = null!;

        /// <summary>
        /// User ID of the safety officer assigned to review/approve the incident.
        /// Optional as not immediately assigned.
        /// </summary>
        public Guid? AssignedToSafetyOfficerId { get; set; }

        /// <summary>
        /// Navigation property for the assigned safety officer.
        /// </summary>
        public ApplicationUser? AssignedToSafetyOfficer { get; set; }

        /// <summary>
        /// Timestamp when the incident was assigned to a safety officer.
        /// </summary>
        public DateTime? AssignedAt { get; set; }

        /// <summary>
        /// User ID of the implementor assigned to fix the incident.
        /// Optional as not immediately assigned.
        /// </summary>
        public Guid? AssignedToImplementorId { get; set; }

        /// <summary>
        /// Navigation property for the assigned implementor.
        /// </summary>
        public ApplicationUser? AssignedToImplementor { get; set; }

        /// <summary>
        /// Timestamp when the incident was assigned to an implementor.
        /// </summary>
        public DateTime? ImplementorAssignedAt { get; set; }

        /// <summary>
        /// Comment from the safety officer or other reviewers.
        /// Used for rejection feedback or additional notes.
        /// </summary>
        public string? ReviewerComment { get; set; }

        /// <summary>
        /// Indicates if this incident has been rejected and requires resubmission.
        /// </summary>
        public bool IsRejected { get; set; } = false;

        /// <summary>
        /// Timestamp when the incident was rejected.
        /// </summary>
        public DateTime? RejectedAt { get; set; }

        /// <summary>
        /// Timestamp when the incident was accepted and approved.
        /// </summary>
        public DateTime? ApprovedAt { get; set; }

        /// <summary>
        /// Timestamp when the incident was closed.
        /// </summary>
        public DateTime? ClosedAt { get; set; }

        /// <summary>
        /// Comment from the safety officer when closing the incident.
        /// </summary>
        public string? ClosureComment { get; set; }

        /// <summary>
        /// Navigation property for comments on this incident.
        /// </summary>
        public List<IncidentComment> Comments { get; set; } = [];

        /// <summary>
        /// Navigation property for the implementation record of this incident.
        /// One-to-one relationship.
        /// </summary>
        public IncidentImplementation? Implementation { get; set; }

        /// <summary>
        /// Navigation property for audit logs related to this incident.
        /// </summary>
        public List<AuditLog> AuditLogs { get; set; } = [];
    }
}