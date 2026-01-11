namespace EHS.Application.DTOs
{
    /// <summary>
    /// Request DTO for creating a Machine.
    /// </summary>
    public class CreateMachineRequest
    {
        public string Name { get; set; } = string.Empty;
        public string EquipmentId { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid ProductionLineId { get; set; }
    }

    /// <summary>
    /// Request DTO for updating a Machine.
    /// </summary>
    public class UpdateMachineRequest
    {
        public string Name { get; set; } = string.Empty;
        public string EquipmentId { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid ProductionLineId { get; set; }
    }

    /// <summary>
    /// Response DTO for Machine.
    /// </summary>
    public class MachineResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EquipmentId { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid ProductionLineId { get; set; }
        public string ProductionLineName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}