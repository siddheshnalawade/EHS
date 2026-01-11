using EHS.Application.DTOs;

namespace EHS.Application.Interfaces
{
    /// <summary>
    /// Service interface for Incident Nature master data operations.
    /// </summary>
    public interface IIncidentNatureService
    {
        Task<ApiResponse<IncidentNatureResponse>> CreateAsync(CreateIncidentNatureRequest request);

        Task<ApiResponse<List<IncidentNatureResponse>>> GetAllAsync();

        Task<ApiResponse<IncidentNatureResponse>> GetByIdAsync(Guid id);

        Task<ApiResponse<IncidentNatureResponse>> UpdateAsync(Guid id, UpdateIncidentNatureRequest request);

        Task<ApiResponse<string>> DeleteAsync(Guid id);
    }
}