namespace EHS.Application.DTOs
{
    /// <summary>
    /// Request to accept an incident for implementation.
    /// </summary>
    public class AcceptIncidentRequest
    {
        public int EstimatedDaysToComplete { get; set; }
    }

    /// <summary>
    /// Request to pass incident to another implementor.
    /// </summary>
    public class PassToPeerRequest
    {
        public Guid PeerImplementorId { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request to update implementation details.
    /// </summary>
    public class UpdateImplementationRequest
    {
        public string RootCauseAnalysis { get; set; } = string.Empty;
        public string CorrectiveActionsDescription { get; set; } = string.Empty;
        public Guid ClosureActionId { get; set; }
        public string? AdditionalRemarks { get; set; }
        public List<Guid> BenefitIds { get; set; } = new();
        public List<RootCauseDetailDto> RootCauseDetails { get; set; } = new();
    }

    /// <summary>
    /// Root cause analysis detail.
    /// </summary>
    public class RootCauseDetailDto
    {
        public string Why { get; set; } = string.Empty;
        public string Analysis { get; set; } = string.Empty;
        public string CorrectiveAction { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response DTO for incident implementation.
    /// </summary>
    public class IncidentImplementationResponse
    {
        public Guid Id { get; set; }
        public Guid IncidentId { get; set; }
        public int EstimatedDaysToComplete { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string RootCauseAnalysis { get; set; } = string.Empty;
        public string CorrectiveActionsDescription { get; set; } = string.Empty;
        public Guid ClosureActionId { get; set; }
        public string ClosureActionName { get; set; } = string.Empty;
        public string? AdditionalRemarks { get; set; }
        public Guid ImplementedByUserId { get; set; }
        public string ImplementedByUserName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<BenefitResponse> Benefits { get; set; } = new();
        public List<RootCauseDetailResponse> RootCauseDetails { get; set; } = new();
    }

    /// <summary>
    /// Response for benefit.
    /// </summary>
    public class BenefitResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    /// <summary>
    /// Response for root cause detail.
    /// </summary>
    public class RootCauseDetailResponse
    {
        public Guid Id { get; set; }
        public string Why { get; set; } = string.Empty;
        public string Analysis { get; set; } = string.Empty;
        public string CorrectiveAction { get; set; } = string.Empty;
    }
}
