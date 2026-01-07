namespace EHS.Domain.Entities
{
    /// <summary>
    /// Represents the severity level of an incident.
    /// </summary>
    public class IncidentSeverity : BaseEntity
    {
        /// <summary>
        /// Name of the severity level (e.g., "Minor", "Major", "Catastrophic").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Numeric value for severity level (1=Minor, 2=Major, 3=Catastrophic) for sorting/comparison.
        /// </summary>
        public int SeverityLevel { get; set; }

        /// <summary>
        /// Description of the severity level.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Navigation property for incidents with this severity.
        /// </summary>
        public List<Incident> Incidents { get; set; } = [];
    }
}