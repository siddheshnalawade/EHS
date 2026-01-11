using EHS.Application.DTOs;

namespace EHS.Application.Interfaces
{
    /// <summary>
    /// Service interface for Production Line master data operations.
    /// </summary>
    public interface IProductionLineService
    {
        Task<ApiResponse<ProductionLineResponse>> CreateAsync(CreateProductionLineRequest request);

        Task<ApiResponse<List<ProductionLineResponse>>> GetAllAsync();

        Task<ApiResponse<List<ProductionLineResponse>>> GetByDepartmentIdAsync(Guid departmentId);

        Task<ApiResponse<ProductionLineResponse>> GetByIdAsync(Guid id);

        Task<ApiResponse<ProductionLineResponse>> UpdateAsync(Guid id, UpdateProductionLineRequest request);

        Task<ApiResponse<string>> DeleteAsync(Guid id);
    }
}