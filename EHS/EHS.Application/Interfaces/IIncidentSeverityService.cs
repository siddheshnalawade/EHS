using EHS.Application.DTOs;

namespace EHS.Application.Interfaces
{
    /// <summary>
    /// Service interface for Incident Severity master data operations.
    /// </summary>
    public interface IIncidentSeverityService
    {
        Task<ApiResponse<IncidentSeverityResponse>> CreateAsync(CreateIncidentSeverityRequest request);

        Task<ApiResponse<List<IncidentSeverityResponse>>> GetAllAsync();

        Task<ApiResponse<IncidentSeverityResponse>> GetByIdAsync(Guid id);

        Task<ApiResponse<IncidentSeverityResponse>> UpdateAsync(Guid id, UpdateIncidentSeverityRequest request);

        Task<ApiResponse<string>> DeleteAsync(Guid id);
    }
}