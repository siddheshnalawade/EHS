using EHS.Application.DTOs;

namespace EHS.Application.Interfaces
{
    /// <summary>
    /// Service interface for Machine master data operations.
    /// </summary>
    public interface IMachineService
    {
        Task<ApiResponse<MachineResponse>> CreateAsync(CreateMachineRequest request);

        Task<ApiResponse<List<MachineResponse>>> GetAllAsync();

        Task<ApiResponse<List<MachineResponse>>> GetByProductionLineIdAsync(Guid productionLineId);

        Task<ApiResponse<MachineResponse>> GetByIdAsync(Guid id);

        Task<ApiResponse<MachineResponse>> UpdateAsync(Guid id, UpdateMachineRequest request);

        Task<ApiResponse<string>> DeleteAsync(Guid id);
    }
}