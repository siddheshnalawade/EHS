namespace EHS.Application.DTOs
{
    /// <summary>
    /// Response DTO for user information
    /// </summary>
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    /// <summary>
    /// Request DTO for assigning a role to a user
    /// </summary>
    public class AssignRoleRequest
    {
        public string RoleName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request DTO for removing a role from a user
    /// </summary>
    public class RemoveRoleRequest
    {
        public string RoleName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response DTO for role information
    /// </summary>
    public class RoleResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
