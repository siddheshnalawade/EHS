namespace EHS.Domain.Entities
{
    /// <summary>
    /// Represents the implementation details and corrective actions for an incident.
    /// This is completed after the incident is approved by the safety officer.
    /// </summary>
    public class IncidentImplementation : BaseEntity
    {
        /// <summary>
        /// Foreign key for the incident being implemented.
        /// One-to-one relationship.
        /// </summary>
        public Guid IncidentId { get; set; }

        /// <summary>
        /// Navigation property for the incident.
        /// </summary>
        public Incident Incident { get; set; } = null!;

        /// <summary>
        /// Estimated timeline for completing the implementation (in days).
        /// </summary>
        public int EstimatedDaysToComplete { get; set; }

        /// <summary>
        /// Timestamp when the implementor accepted the incident for implementation.
        /// </summary>
        public DateTime? AcceptedAt { get; set; }

        /// <summary>
        /// Timestamp when the implementor started working on the incident.
        /// </summary>
        public DateTime? StartedAt { get; set; }

        /// <summary>
        /// Timestamp when the implementation was completed.
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// Root cause analysis for the incident.
        /// </summary>
        public string RootCauseAnalysis { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of corrective actions taken.
        /// </summary>
        public string CorrectiveActionsDescription { get; set; } = string.Empty;

        /// <summary>
        /// Foreign key for the closure action type selected.
        /// </summary>
        public Guid ClosureActionId { get; set; }

        /// <summary>
        /// Navigation property for the closure action.
        /// </summary>
        public ClosureAction ClosureAction { get; set; } = null!;

        /// <summary>
        /// Additional remarks or notes about the implementation.
        /// </summary>
        public string? AdditionalRemarks { get; set; }

        /// <summary>
        /// User ID of the implementor who accepted and completed the work.
        /// </summary>
        public Guid ImplementedByUserId { get; set; }

        /// <summary>
        /// Navigation property for the implementor user.
        /// </summary>
        public ApplicationUser ImplementedByUser { get; set; } = null!;

        /// <summary>
        /// Status of the implementation (e.g., "Pending", "InProgress", "Completed", "VerificationPending").
        /// </summary>
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Navigation property for benefits identified from this implementation.
        /// </summary>
        public List<ImplementationBenefit> Benefits { get; set; } = [];

        /// <summary>
        /// Navigation property for root cause details.
        /// </summary>
        public List<RootCauseAnalysisDetail> RootCauseDetails { get; set; } = [];
    }
}