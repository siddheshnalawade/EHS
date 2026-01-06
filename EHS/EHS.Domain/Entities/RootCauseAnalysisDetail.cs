namespace EHS.Domain.Entities
{
    /// <summary>
    /// Represents detailed root cause factors for an incident implementation.
    /// Allows tracking multiple root causes and corresponding corrective measures.
    /// </summary>
    public class RootCauseAnalysisDetail : BaseEntity
    {
        /// <summary>
        /// The identified root cause or contributing factor.
        /// </summary>
        public string RootCause { get; set; } = string.Empty;

        /// <summary>
        /// The corrective measure or action taken to address this root cause.
        /// </summary>
        public string CorrectiveMeasure { get; set; } = string.Empty;

        /// <summary>
        /// Person or department responsible for implementing the corrective measure.
        /// </summary>
        public string ResponsibleParty { get; set; } = string.Empty;

        /// <summary>
        /// Expected date for completing the corrective measure.
        /// </summary>
        public DateTime? TargetCompletionDate { get; set; }

        /// <summary>
        /// Actual date when the corrective measure was completed.
        /// </summary>
        public DateTime? ActualCompletionDate { get; set; }

        /// <summary>
        /// Status of the corrective action (e.g., "NotStarted", "InProgress", "Completed", "Verified").
        /// </summary>
        public string Status { get; set; } = "NotStarted";

        /// <summary>
        /// Foreign key for the implementation this detail belongs to.
        /// </summary>
        public Guid ImplementationId { get; set; }

        /// <summary>
        /// Navigation property for the implementation.
        /// </summary>
        public IncidentImplementation Implementation { get; set; } = null!;
    }
}