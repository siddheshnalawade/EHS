namespace EHS.Domain.Entities
{
    /// <summary>
    /// Represents a comment or note added to an incident by various stakeholders.
    /// Enables communication between initiator, safety officer, and implementor.
    /// </summary>
    public class IncidentComment : BaseEntity
    {
        /// <summary>
        /// The comment text content.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Foreign key for the incident this comment belongs to.
        /// </summary>
        public Guid IncidentId { get; set; }

        /// <summary>
        /// Navigation property for the incident.
        /// </summary>
        public Incident Incident { get; set; } = null!;

        /// <summary>
        /// User ID of the person who wrote the comment.
        /// </summary>
        public Guid CommentedByUserId { get; set; }

        /// <summary>
        /// Navigation property for the user who commented.
        /// </summary>
        public ApplicationUser CommentedByUser { get; set; } = null!;

        /// <summary>
        /// Role or capacity of the commenter (e.g., "Initiator", "SafetyOfficer", "Implementor").
        /// </summary>
        public string CommentedAsRole { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if this comment is internal/private or visible to all stakeholders.
        /// </summary>
        public bool IsInternal { get; set; } = false;
    }
}