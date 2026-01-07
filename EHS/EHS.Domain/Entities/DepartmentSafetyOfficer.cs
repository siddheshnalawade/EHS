namespace EHS.Domain.Entities
{
    /// <summary>
    /// Junction entity linking safety officers to departments they manage.
    /// A safety officer can manage multiple departments, and departments can have multiple safety officers.
    /// </summary>
    public class DepartmentSafetyOfficer : BaseEntity
    {
        /// <summary>
        /// Foreign key for the department.
        /// </summary>
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// Navigation property for the department.
        /// </summary>
        public Department Department { get; set; } = null!;

        /// <summary>
        /// Foreign key for the safety officer user.
        /// </summary>
        public Guid SafetyOfficerId { get; set; }

        /// <summary>
        /// Navigation property for the safety officer user.
        /// </summary>
        public ApplicationUser SafetyOfficer { get; set; } = null!;

        /// <summary>
        /// Date when the safety officer was assigned to this department.
        /// </summary>
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date when the safety officer assignment ended (null if still active).
        /// </summary>
        public DateTime? UnassignedDate { get; set; }

        /// <summary>
        /// Indicates if this assignment is currently active.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}