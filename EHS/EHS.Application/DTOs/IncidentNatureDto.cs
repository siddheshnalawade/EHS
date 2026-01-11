namespace EHS.Application.DTOs
{
    /// <summary>
    /// Request DTO for creating an Incident Nature.
    /// </summary>
    public class CreateIncidentNatureRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    /// <summary>
    /// Request DTO for updating an Incident Nature.
    /// </summary>
    public class UpdateIncidentNatureRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    /// <summary>
    /// Response DTO for Incident Nature.
    /// </summary>
    public class IncidentNatureResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}