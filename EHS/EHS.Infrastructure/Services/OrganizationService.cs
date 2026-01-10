using AutoMapper;
using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using EHS.Application.Repositories;
using EHS.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EHS.Infrastructure.Services
{
    public class OrganizationService(
        IRepository<Organization> organizationRepository,
        IMapper mapper,
        ILogger<OrganizationService> logger
        ) : IOrganizationService
    {
        public async Task<ApiResponse<OrganizationResponse>> CreateOrganizationAsync(CreateOrganizationRequest request)
        {
            try
            {
                // Map request to Organization entity
                var organization = mapper.Map<Organization>(request);

                // Add organization to repository
                await organizationRepository.AddAsync(organization);

                // Map entity to response DTO
                var organizationResponse = mapper.Map<OrganizationResponse>(organization);

                logger.LogInformation("Organization created successfully. OrganizationId: {OrganizationId}, Name: {Name}",
                  organization.Id, organization.Name);

                return new ApiResponse<OrganizationResponse>
                {
                    IsSuccessful = true,
                    Data = organizationResponse,
                    Message = "Organization created successfully."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating organization. Request: {@Request}", request);
                return new ApiResponse<OrganizationResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while creating the organization."
                };
            }
        }

        public async Task<ApiResponse<string>> DeleteOrganizationAsync(Guid id)
        {
            try
            {
                // Validate ID
                if (id == Guid.Empty)
                {
                    logger.LogWarning("Invalid organization ID provided for deletion: {Id}", id);
                    return new ApiResponse<string>
                    {
                        IsSuccessful = false,
                        Message = "Invalid organization ID."
                    };
                }

                // Soft delete organization
                var deleted = await organizationRepository.DeleteAsync(id);

                if (!deleted)
                {
                    logger.LogWarning("Organization not found for deletion. OrganizationId: {Id}", id);
                    return new ApiResponse<string>
                    {
                        IsSuccessful = false,
                        Message = "Organization not found."
                    };
                }

                // Persist changes
                await organizationRepository.SaveChangesAsync();

                logger.LogInformation("Organization deleted successfully. OrganizationId: {Id}", id);

                return new ApiResponse<string>
                {
                    IsSuccessful = true,
                    Message = "Organization deleted successfully."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting organization. OrganizationId: {Id}", id);
                return new ApiResponse<string>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while deleting the organization."
                };
            }
        }

        public async Task<ApiResponse<PaginatedResponse<OrganizationResponse>>> GetAllOrganizationsAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            try
            {
                // Build filter predicate
                var filter = BuildOrganizationFilter(searchTerm);

                // Get paginated results with related entities
                var (organizations, totalCount) = await organizationRepository.GetPagedAsync(
                    pageNumber,
                    pageSize,
                    filter,
                    q => q.OrderByDescending(o => o.CreatedAt),
                    o => o.Departments,
                    o => o.Incidents);

                // Map entities to DTOs
                var organizationDtos = mapper.Map<List<OrganizationResponse>>(organizations);

                var paginatedResponse = new PaginatedResponse<OrganizationResponse>
                {
                    Items = organizationDtos,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };

                logger.LogInformation("Retrieved {Count} organizations. Page: {PageNumber}, Total: {TotalCount}",
                    organizationDtos.Count, pageNumber, totalCount);

                return new ApiResponse<PaginatedResponse<OrganizationResponse>>
                {
                    IsSuccessful = true,
                    Message = "Organizations retrieved successfully.",
                    Data = paginatedResponse
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving organizations. SearchTerm: {SearchTerm}, Page: {PageNumber}",
                    searchTerm, pageNumber);
                return new ApiResponse<PaginatedResponse<OrganizationResponse>>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while retrieving organizations."
                };
            }
        }

        public async Task<ApiResponse<OrganizationResponse>> GetOrganizationByIdAsync(Guid id)
        {
            try
            {
                // Validate ID
                if (id == Guid.Empty)
                {
                    logger.LogWarning("Invalid organization ID provided: {Id}", id);
                    return new ApiResponse<OrganizationResponse>
                    {
                        IsSuccessful = false,
                        Message = "Invalid organization ID."
                    };
                }

                var organization = await organizationRepository.GetByIdAsync(
                    id,
                    o => o.Departments,
                    o => o.Incidents);

                if (organization is null)
                {
                    return new ApiResponse<OrganizationResponse>
                    {
                        IsSuccessful = false,
                        Message = "Organization not found."
                    };
                }

                var resposne = mapper.Map<OrganizationResponse>(organization);
                return new ApiResponse<OrganizationResponse>
                {
                    IsSuccessful = true,
                    Message = "Organization retrieved successfully.",
                    Data = resposne
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving organization by ID: {OrganizationId}", id);
                return new ApiResponse<OrganizationResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while retrieving the organization."
                };
            }
        }

        public Task<ApiResponse<OrganizationStatisticsResponse>> GetOrganizationStatisticsAsync(Guid id)
        {
            return Task.FromResult(new ApiResponse<OrganizationStatisticsResponse>
            {
                IsSuccessful = false,
                Message = "Not implemented yet."
            });
        }

        public async Task<ApiResponse<OrganizationResponse>> UpdateOrganizationAsync(Guid id, UpdateOrganizationRequest request)
        {
            try
            {
                // Validate ID
                if (id == Guid.Empty)
                {
                    logger.LogWarning("Invalid organization ID provided for update: {Id}", id);
                    return new ApiResponse<OrganizationResponse>
                    {
                        IsSuccessful = false,
                        Message = "Invalid organization ID."
                    };
                }

                var organization = await organizationRepository.GetByIdAsync(id);

                if (organization is null)
                {
                    logger.LogWarning("Organization not found for update. OrganizationId: {Id}", id);
                    return new ApiResponse<OrganizationResponse>
                    {
                        IsSuccessful = false,
                        Message = "Organization not found."
                    };
                }

                mapper.Map(request, organization);

                organizationRepository.UpdateAsync(organization);

                await organizationRepository.SaveChangesAsync();

                // Retrieve updated organization with related entities
                var updatedOrganization = await organizationRepository.GetByIdAsync(
                    id,
                    o => o.Departments,
                    o => o.Incidents);

                // Map entity to response DTO
                var response = mapper.Map<OrganizationResponse>(updatedOrganization);

                logger.LogInformation("Organization updated successfully. OrganizationId: {Id}", id);

                return new ApiResponse<OrganizationResponse>
                {
                    IsSuccessful = true,
                    Message = "Organization updated successfully.",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating organization. OrganizationId: {Id}, Request: {@Request}", id, request);
                return new ApiResponse<OrganizationResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while updating the organization."
                };
            }
        }

        private Expression<Func<Organization, bool>> BuildOrganizationFilter(string? searchTerm)
        {
            return o => string.IsNullOrWhiteSpace(searchTerm) ||
            o.Name.ToLower().Contains(searchTerm.ToLower()) ||
            o.City.ToLower().Contains(searchTerm.ToLower());
        }
    }
}