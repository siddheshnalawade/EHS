namespace EHS.Application.DTOs
{
    /// <summary>
    /// Request DTO for creating an Incident Severity.
    /// </summary>
    public class CreateIncidentSeverityRequest
    {
        public string Name { get; set; } = string.Empty;
        public int SeverityLevel { get; set; }
        public string? Description { get; set; }
    }

    /// <summary>
    /// Request DTO for updating an Incident Severity.
    /// </summary>
    public class UpdateIncidentSeverityRequest
    {
        public string Name { get; set; } = string.Empty;
        public int SeverityLevel { get; set; }
        public string? Description { get; set; }
    }

    /// <summary>
    /// Response DTO for Incident Severity.
    /// </summary>
    public class IncidentSeverityResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SeverityLevel { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}