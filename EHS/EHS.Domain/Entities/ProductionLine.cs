namespace EHS.Domain.Entities
{
    /// <summary>
    /// Represents a production line or cell within a department.
    /// </summary>
    public class ProductionLine : BaseEntity
    {
        /// <summary>
        /// Name of the production line or cell.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Short code for the production line (e.g., "L1", "C2").
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Description of the production line's function.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Supervisor or lead of the production line.
        /// </summary>
        public string SupervisorName { get; set; } = string.Empty;

        /// <summary>
        /// Foreign key referencing the parent department.
        /// </summary>
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// Navigation property for the parent department.
        /// </summary>
        public Department Department { get; set; } = null!;

        /// <summary>
        /// Navigation property for machines on this production line.
        /// </summary>
        public List<Machine> Machines { get; set; } = [];

        /// <summary>
        /// Navigation property for incidents on this production line.
        /// </summary>
        public List<Incident> Incidents { get; set; } = [];
    }
}