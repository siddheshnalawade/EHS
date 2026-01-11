namespace EHS.Application.DTOs
{
    /// <summary>
    /// Request DTO for creating a Department.
    /// </summary>
    public class CreateDepartmentRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ManagerName { get; set; }
        public string? Email { get; set; }
        public Guid OrganizationId { get; set; }
    }

    /// <summary>
    /// Request DTO for updating a Department.
    /// </summary>
    public class UpdateDepartmentRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ManagerName { get; set; }
        public string? Email { get; set; }
        public Guid OrganizationId { get; set; }
    }

    /// <summary>
    /// Response DTO for Department.
    /// </summary>
    public class DepartmentResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ManagerName { get; set; }
        public string? Email { get; set; }
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public int ProductionLineCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}