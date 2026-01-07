namespace EHS.Domain.Entities
{
    /// <summary>
    /// Represents the current status of an incident in the workflow.
    /// </summary>
    public class IncidentStatus : BaseEntity
    {
        /// <summary>
        /// Name of the status (e.g., "Submitted", "Assigned", "InProgress", "Closed", "Rejected").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Numeric value representing the workflow stage for ordering.
        /// </summary>
        public int StageOrder { get; set; }

        /// <summary>
        /// Description of the status and its meaning in the workflow.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Navigation property for incidents with this status.
        /// </summary>
        public List<Incident> Incidents { get; set; } = [];
    }
}