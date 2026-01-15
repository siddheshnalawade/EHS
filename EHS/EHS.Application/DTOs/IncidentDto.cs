namespace EHS.Application.DTOs
{
    /// <summary>
    /// Request DTO for creating an incident.
    /// </summary>
    public class CreateIncidentRequest
    {
        public Guid IncidentTypeId { get; set; }
        public Guid IncidentNatureId { get; set; }
        public Guid IncidentSeverityId { get; set; }
        public DateTime IncidentDate { get; set; }
        public TimeSpan? IncidentTime { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid? ProductionLineId { get; set; }
        public Guid? MachineId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IncidentArea { get; set; } = string.Empty;
        public string ProposedSolution { get; set; } = string.Empty;
        public List<Microsoft.AspNetCore.Http.IFormFile>? EvidenceFiles { get; set; }
    }

    /// <summary>
    /// Request DTO for updating an incident (by initiator).
    /// </summary>
    public class UpdateIncidentRequest
    {
        public Guid IncidentTypeId { get; set; }
        public Guid IncidentNatureId { get; set; }
        public Guid IncidentSeverityId { get; set; }
        public DateTime IncidentDate { get; set; }
        public TimeSpan? IncidentTime { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid? ProductionLineId { get; set; }
        public Guid? MachineId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IncidentArea { get; set; } = string.Empty;
        public string ProposedSolution { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response DTO for incident details.
    /// </summary>
    public class IncidentResponse
    {
        public Guid Id { get; set; }
        public string IncidentNumber { get; set; } = string.Empty;

        // Type, Nature, Severity
        public Guid IncidentTypeId { get; set; }

        public string IncidentTypeName { get; set; } = string.Empty;
        public Guid IncidentNatureId { get; set; }
        public string IncidentNatureName { get; set; } = string.Empty;
        public Guid IncidentSeverityId { get; set; }
        public string IncidentSeverityName { get; set; } = string.Empty;
        public int SeverityLevel { get; set; }

        // Status
        public Guid IncidentStatusId { get; set; }

        public string IncidentStatusName { get; set; } = string.Empty;

        // Incident Details
        public DateTime IncidentDate { get; set; }

        public TimeSpan? IncidentTime { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IncidentArea { get; set; } = string.Empty;
        public string ProposedSolution { get; set; } = string.Empty;

        // Location
        public Guid OrganizationId { get; set; }

        public string OrganizationName { get; set; } = string.Empty;
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public Guid? ProductionLineId { get; set; }
        public string? ProductionLineName { get; set; }
        public Guid? MachineId { get; set; }
        public string? MachineName { get; set; }

        // Users
        public Guid InitiatedByUserId { get; set; }

        public string InitiatedByUserName { get; set; } = string.Empty;
        public Guid? AssignedToSafetyOfficerId { get; set; }
        public string? AssignedToSafetyOfficerName { get; set; }
        public Guid? AssignedToImplementorId { get; set; }
        public string? AssignedToImplementorName { get; set; }

        // Timestamps
        public DateTime CreatedAt { get; set; }

        public DateTime? AssignedAt { get; set; }
        public DateTime? ImplementorAssignedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? RejectedAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        // Comments
        public string? ReviewerComment { get; set; }

        public string? ClosureComment { get; set; }
        public bool IsRejected { get; set; }

        // Implementation
        public IncidentImplementationResponse? Implementation { get; set; }

        public List<IncidentAttachmentResponse> Attachments { get; set; } = [];
    }

    /// <summary>
    /// Simplified incident list response.
    /// </summary>
    public class IncidentListResponse
    {
        public Guid Id { get; set; }
        public string IncidentNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string IncidentTypeName { get; set; } = string.Empty;
        public string IncidentSeverityName { get; set; } = string.Empty;
        public int SeverityLevel { get; set; }
        public string IncidentStatusName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public DateTime IncidentDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string InitiatedByUserName { get; set; } = string.Empty;
        public string? AssignedToSafetyOfficerName { get; set; }
        public string? AssignedToImplementorName { get; set; }
    }

    /// <summary>
    /// Filter request for incidents.
    /// </summary>
    public class IncidentFilterRequest
    {
        public Guid? OrganizationId { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? IncidentTypeId { get; set; }
        public Guid? IncidentSeverityId { get; set; }
        public Guid? IncidentStatusId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}