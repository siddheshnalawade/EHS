using EHS.Application.DTOs;

namespace EHS.Application.Interfaces
{
    /// <summary>
    /// Service interface for Incident Type master data operations.
    /// </summary>
    public interface IIncidentTypeService
    {
        Task<ApiResponse<IncidentTypeResponse>> CreateAsync(CreateIncidentTypeRequest request);

        Task<ApiResponse<List<IncidentTypeResponse>>> GetAllAsync();

        Task<ApiResponse<IncidentTypeResponse>> GetByIdAsync(Guid id);

        Task<ApiResponse<IncidentTypeResponse>> UpdateAsync(Guid id, UpdateIncidentTypeRequest request);

        Task<ApiResponse<string>> DeleteAsync(Guid id);
    }
}