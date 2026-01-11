using EHS.Application.DTOs;

namespace EHS.Application.Interfaces
{
    /// <summary>
    /// Service interface for Department master data operations.
    /// </summary>
    public interface IDepartmentService
    {
        Task<ApiResponse<DepartmentResponse>> CreateAsync(CreateDepartmentRequest request);

        Task<ApiResponse<PaginatedResponse<DepartmentResponse>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null);

        Task<ApiResponse<List<DepartmentResponse>>> GetByOrganizationIdAsync(Guid organizationId);

        Task<ApiResponse<DepartmentResponse>> GetByIdAsync(Guid id);

        Task<ApiResponse<DepartmentResponse>> UpdateAsync(Guid id, UpdateDepartmentRequest request);

        Task<ApiResponse<string>> DeleteAsync(Guid id);
    }
}