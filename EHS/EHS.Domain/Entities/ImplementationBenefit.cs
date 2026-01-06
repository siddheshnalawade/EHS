namespace EHS.Domain.Entities
{
    /// <summary>
    /// Junction entity linking implementations to benefits.
    /// Allows tracking multiple benefits from a single implementation.
    /// </summary>
    public class ImplementationBenefit : BaseEntity
    {
        /// <summary>
        /// Foreign key for the implementation.
        /// </summary>
        public Guid ImplementationId { get; set; }

        /// <summary>
        /// Navigation property for the implementation.
        /// </summary>
        public IncidentImplementation Implementation { get; set; } = null!;

        /// <summary>
        /// Foreign key for the benefit.
        /// </summary>
        public Guid BenefitId { get; set; }

        /// <summary>
        /// Navigation property for the benefit.
        /// </summary>
        public Benefit Benefit { get; set; } = null!;

        /// <summary>
        /// Quantifiable measure or value of the benefit (if applicable).
        /// E.g., "15% reduction in injury rate", "2 hours saved per shift".
        /// </summary>
        public string? BenefitValue { get; set; }
    }
}