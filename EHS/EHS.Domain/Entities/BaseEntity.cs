namespace EHS.Domain.Entities
{
    /// <summary>
    /// Base entity for all domain entities.
    /// Provides common properties for entity tracking and auditing.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Unique identifier for the entity.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Timestamp when the entity was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// User ID of who created the entity.
        /// </summary>
        public Guid? CreatedBy { get; set; }

        /// <summary>
        /// Timestamp when the entity was last modified.
        /// </summary>
        public DateTime? ModifiedAt { get; set; }

        /// <summary>
        /// User ID of who last modified the entity.
        /// </summary>
        public Guid? ModifiedBy { get; set; }

        /// <summary>
        /// Soft delete indicator. True if the entity has been deleted.
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Timestamp when the entity was deleted (soft delete).
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// User ID of who deleted the entity.
        /// </summary>
        public Guid? DeletedBy { get; set; }
    }
}