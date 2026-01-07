namespace EHS.Domain.Entities
{
    /// <summary>
    /// Represents whether an incident is new or repeated.
    /// </summary>
    public class IncidentNature : BaseEntity
    {
        /// <summary>
        /// Name of the nature (e.g., "New", "Repeated").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description explaining the nature.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Navigation property for incidents with this nature.
        /// </summary>
        public List<Incident> Incidents { get; set; } = [];
    }
}