namespace EHS.Domain.Entities
{
    /// <summary>
    /// Represents the closure action taken to address an incident's root cause.
    /// </summary>
    public class ClosureAction : BaseEntity
    {
        /// <summary>
        /// Name of the closure action (e.g., "Engineering Control", "Administrative Control", "Training").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of the closure action type.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Navigation property for implementation actions using this closure action.
        /// </summary>
        public List<IncidentImplementation> Implementations { get; set; } = [];
    }
}