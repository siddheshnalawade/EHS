using Microsoft.AspNetCore.Identity;

namespace EHS.Domain.Entities
{
    /// <summary>
    /// Represents an application user with enhanced security properties for the EHS system.
    /// </summary>
    public class ApplicationUser : IdentityUser<Guid>
    {
        /// <summary>
        /// Full name of the user (e.g., "John Doe").
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Collection of active and revoked refresh tokens for token rotation and management.
        /// Tokens are automatically validated based on IsActive property.
        /// </summary>
        public List<RefreshToken>? RefreshTokens { get; set; } = [];

        /// <summary>
        /// Timestamp when the user account was created.
        /// Used for audit trails and security monitoring.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when the user was last active.
        /// Useful for detecting inactive accounts and security auditing.
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Timestamp when the password was last changed.
        /// Used to enforce periodic password changes if required.
        /// </summary>
        public DateTime? LastPasswordChangedAt { get; set; }

        /// <summary>
        /// Indicates if the user account is active/enabled.
        /// Allows soft-delete functionality without removing data.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}