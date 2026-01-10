using EHS.Application.DTOs;

namespace EHS.Application.Interfaces
{
    /// <summary>
    /// Service interface for organization management operations.
    /// </summary>
    public interface IOrganizationService
    {
        /// <summary>
        /// Creates a new organization (Admin only).
        /// </summary>
        /// <param name="request">Organization creation request</param>
        /// <returns>Created organization response</returns>
        Task<ApiResponse<OrganizationResponse>> CreateOrganizationAsync(CreateOrganizationRequest request);

        /// <summary>
        /// Retrieves all organizations with pagination.
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Items per page</param>
        /// <param name="searchTerm">Optional search term for name or city</param>
        /// <returns>Paginated list of organizations</returns>
        Task<ApiResponse<PaginatedResponse<OrganizationResponse>>> GetAllOrganizationsAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null);

        /// <summary>
        /// Retrieves a specific organization by ID.
        /// </summary>
        /// <param name="id">Organization ID</param>
        /// <returns>Organization details</returns>
        Task<ApiResponse<OrganizationResponse>> GetOrganizationByIdAsync(Guid id);

        /// <summary>
        /// Updates an existing organization (Admin only).
        /// </summary>
        /// <param name="id">Organization ID</param>
        /// <param name="request">Update request</param>
        /// <returns>Updated organization response</returns>
        Task<ApiResponse<OrganizationResponse>> UpdateOrganizationAsync(Guid id, UpdateOrganizationRequest request);

        /// <summary>
        /// Soft deletes an organization (Admin only).
        /// </summary>
        /// <param name="id">Organization ID</param>
        /// <returns>Success message</returns>
        Task<ApiResponse<string>> DeleteOrganizationAsync(Guid id);

        /// <summary>
        /// Gets statistics for an organization.
        /// </summary>
        /// <param name="id">Organization ID</param>
        /// <returns>Organization statistics</returns>
        Task<ApiResponse<OrganizationStatisticsResponse>> GetOrganizationStatisticsAsync(Guid id);
    }
}