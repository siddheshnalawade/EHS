using EHS.Application.DTOs;

namespace EHS.Application.Interfaces
{
    /// <summary>
    /// Service interface for user management operations.
    /// </summary>
    public interface IUserManagementService
    {
        Task<ApiResponse<PaginatedResponse<UserResponse>>> GetUsersAsync(int pageNumber, int pageSize, string? searchTerm);
        Task<ApiResponse<UserResponse>> GetUserByIdAsync(Guid userId);
        Task<ApiResponse<UserResponse>> AssignRoleAsync(Guid userId, string roleName);
        Task<ApiResponse<UserResponse>> RemoveRoleAsync(Guid userId, string roleName);
        Task<ApiResponse<UserResponse>> ToggleUserStatusAsync(Guid userId);
        Task<ApiResponse<List<RoleResponse>>> GetRolesAsync();
    }
}
