namespace EHS.Application.DTOs
{
    /// <summary>
    /// Request DTO for creating an Incident Type.
    /// </summary>
    public class CreateIncidentTypeRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    /// <summary>
    /// Request DTO for updating an Incident Type.
    /// </summary>
    public class UpdateIncidentTypeRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    /// <summary>
    /// Response DTO for Incident Type.
    /// </summary>
    public class IncidentTypeResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}