namespace EHS.Domain.Entities
{
    /// <summary>
    /// Represents a department within an organization.
    /// </summary>
    public class Department : BaseEntity
    {
        /// <summary>
        /// Name of the department.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Short code for the department (e.g., "QA", "PROD", "HR").
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Description of the department's responsibilities.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Manager or head of the department.
        /// </summary>
        public string ManagerName { get; set; } = string.Empty;

        /// <summary>
        /// Contact email for the department.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Foreign key referencing the parent organization.
        /// </summary>
        public Guid OrganizationId { get; set; }

        /// <summary>
        /// Navigation property for the parent organization.
        /// </summary>
        public Organization Organization { get; set; } = null!;

        /// <summary>
        /// Navigation property for production lines/cells within this department.
        /// </summary>
        public List<ProductionLine> ProductionLines { get; set; } = [];

        /// <summary>
        /// Navigation property for incidents in this department.
        /// </summary>
        public List<Incident> Incidents { get; set; } = [];

        /// <summary>
        /// Navigation property for safety officers assigned to this department.
        /// </summary>
        public List<DepartmentSafetyOfficer> SafetyOfficers { get; set; } = [];
    }
}