using EHS.Application.DTOs;

namespace EHS.Application.Interfaces
{
    /// <summary>
    /// Service interface for incident management operations.
    /// </summary>
    public interface IIncidentService
    {
        // Initiator operations
        Task<ApiResponse<IncidentResponse>> CreateIncidentAsync(CreateIncidentRequest request, Guid userId);

        Task<ApiResponse<PaginatedResponse<IncidentListResponse>>> GetIncidentsAsync(IncidentFilterRequest filter);

        Task<ApiResponse<IncidentResponse>> GetIncidentByIdAsync(Guid id);

        Task<ApiResponse<IncidentResponse>> UpdateIncidentAsync(Guid id, UpdateIncidentRequest request, Guid userId);

        Task<ApiResponse<string>> DeleteIncidentAsync(Guid id, Guid userId);

        Task<ApiResponse<PaginatedResponse<IncidentListResponse>>> GetMyIncidentsAsync(Guid userId, int pageNumber = 1, int pageSize = 10);

        // Safety Officer operations
        Task<ApiResponse<IncidentResponse>> AssignToSafetyOfficerAsync(Guid incidentId, Guid safetyOfficerId, Guid userId);

        Task<ApiResponse<IncidentResponse>> RejectIncidentAsync(Guid incidentId, RejectIncidentRequest request, Guid userId);

        Task<ApiResponse<IncidentResponse>> ReassignToInitiatorAsync(Guid incidentId, ReassignToInitiatorRequest request, Guid userId);

        Task<ApiResponse<IncidentResponse>> AcceptAndAssignAsync(Guid incidentId, AcceptAndAssignRequest request, Guid userId);

        Task<ApiResponse<PaginatedResponse<IncidentListResponse>>> GetPendingReviewAsync(Guid safetyOfficerId, int pageNumber = 1, int pageSize = 10);

        Task<ApiResponse<IncidentResponse>> CloseIncidentAsync(Guid incidentId, CloseIncidentRequest request, Guid userId);

        // Implementor operations
        Task<ApiResponse<IncidentResponse>> AcceptIncidentAsync(Guid incidentId, AcceptIncidentRequest request, Guid userId);

        Task<ApiResponse<IncidentResponse>> PassToPeerAsync(Guid incidentId, PassToPeerRequest request, Guid userId);

        Task<ApiResponse<IncidentResponse>> UpdateImplementationAsync(Guid incidentId, UpdateImplementationRequest request, Guid userId);

        Task<ApiResponse<PaginatedResponse<IncidentListResponse>>> GetMyImplementationsAsync(Guid implementorId, int pageNumber = 1, int pageSize = 10);

        Task<ApiResponse<PaginatedResponse<IncidentListResponse>>> GetPendingVerificationAsync(Guid safetyOfficerId, int pageNumber = 1, int pageSize = 10);
    }
}