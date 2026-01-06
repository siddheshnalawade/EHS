namespace EHS.Domain.Entities
{
    /// <summary>
    /// Represents the type of incident in the EHS system.
    /// </summary>
    public class IncidentType : BaseEntity
    {
        /// <summary>
        /// Name of the incident type (e.g., "Near Miss", "Unsafe Condition", "Unsafe Action").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the incident type.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Navigation property for incidents of this type.
        /// </summary>
        public List<Incident> Incidents { get; set; } = [];
    }
}