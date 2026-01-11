namespace EHS.Application.DTOs
{
    /// <summary>
    /// Request DTO for creating a Production Line.
    /// </summary>
    public class CreateProductionLineRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? SupervisorName { get; set; }
        public Guid DepartmentId { get; set; }
    }

    /// <summary>
    /// Request DTO for updating a Production Line.
    /// </summary>
    public class UpdateProductionLineRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? SupervisorName { get; set; }
        public Guid DepartmentId { get; set; }
    }

    /// <summary>
    /// Response DTO for Production Line.
    /// </summary>
    public class ProductionLineResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? SupervisorName { get; set; }
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int MachineCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}