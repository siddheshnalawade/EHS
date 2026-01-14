namespace EHS.Application.DTOs
{
    /// <summary>
    /// Request to assign incident to safety officer.
    /// </summary>
    public class AssignToSafetyOfficerRequest
    {
        public Guid SafetyOfficerId { get; set; }
    }

    /// <summary>
    /// Request to reject an incident.
    /// </summary>
    public class RejectIncidentRequest
    {
        public string Comment { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request to reassign incident to initiator for modifications.
    /// </summary>
    public class ReassignToInitiatorRequest
    {
        public string Comment { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request to accept and assign incident to implementor.
    /// </summary>
    public class AcceptAndAssignRequest
    {
        public Guid ImplementorId { get; set; }
        public string? Comment { get; set; }
    }

    /// <summary>
    /// Request to close an incident.
    /// </summary>
    public class CloseIncidentRequest
    {
        public string ClosureComment { get; set; } = string.Empty;
    }
}