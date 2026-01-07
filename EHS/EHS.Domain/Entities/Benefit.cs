namespace EHS.Domain.Entities
{
    /// <summary>
    /// Represents the benefits or improvements resulting from incident implementation.
    /// </summary>
    public class Benefit : BaseEntity
    {
        /// <summary>
        /// Name of the benefit (e.g., "Reduced Injury Rate", "Improved Process Efficiency").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of the benefit.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Category of the benefit (e.g., "Safety", "Quality", "Efficiency").
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Navigation property for implementations realizing this benefit.
        /// </summary>
        public List<ImplementationBenefit> Implementations { get; set; } = [];
    }
}