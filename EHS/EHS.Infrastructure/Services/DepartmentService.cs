using AutoMapper;
using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using EHS.Application.Repositories;
using EHS.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EHS.Infrastructure.Services
{
    public class DepartmentService(
        IRepository<Department> _repository,
        IRepository<Organization> _organizationRepository,
        IMapper _mapper,
        ILogger<DepartmentService> _logger) : IDepartmentService
    {
        public async Task<ApiResponse<DepartmentResponse>> CreateAsync(CreateDepartmentRequest request)
        {
            try
            {
                // Validate organization exists
                var organizationExists = await _organizationRepository.ExistsAsync(x => x.Id == request.OrganizationId);
                if (!organizationExists)
                {
                    return new ApiResponse<DepartmentResponse>
                    {
                        IsSuccessful = false,
                        Message = "Organization not found."
                    };
                }

                // Check if department code already exists in this organization
                var codeExists = await _repository.ExistsAsync(x =>
                    x.Code.ToLower() == request.Code.ToLower() &&
                    x.OrganizationId == request.OrganizationId);

                if (codeExists)
                {
                    return new ApiResponse<DepartmentResponse>
                    {
                        IsSuccessful = false,
                        Message = "A department with this code already exists in the organization."
                    };
                }

                var department = _mapper.Map<Department>(request);
                await _repository.AddAsync(department);
                await _repository.SaveChangesAsync();

                // Retrieve with navigation properties
                var createdDepartment = await _repository.GetByIdAsync(
                    department.Id,
                    d => d.Organization,
                    d => d.ProductionLines
                );

                _logger.LogInformation("Department created: {Code} in Organization {OrgId}",
                    department.Code, department.OrganizationId);

                return new ApiResponse<DepartmentResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<DepartmentResponse>(createdDepartment),
                    Message = "Department created successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating department");
                return new ApiResponse<DepartmentResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while creating the department."
                };
            }
        }

        public async Task<ApiResponse<PaginatedResponse<DepartmentResponse>>> GetAllAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null)
        {
            try
            {
                var (departments, totalCount) = await _repository.GetPagedAsync(
                    pageNumber,
                    pageSize,
                    filter: string.IsNullOrWhiteSpace(searchTerm) ? null :
                        d => d.Name.Contains(searchTerm) || d.Code.Contains(searchTerm),
                    orderBy: q => q.OrderBy(d => d.Name),
                    d => d.Organization,
                    d => d.ProductionLines
                );

                var departmentDtos = _mapper.Map<List<DepartmentResponse>>(departments);

                return new ApiResponse<PaginatedResponse<DepartmentResponse>>
                {
                    IsSuccessful = true,
                    Data = new PaginatedResponse<DepartmentResponse>
                    {
                        Items = departmentDtos,
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalCount = totalCount
                    },
                    Message = "Departments retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving departments");
                return new ApiResponse<PaginatedResponse<DepartmentResponse>>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while retrieving departments."
                };
            }
        }

        public async Task<ApiResponse<List<DepartmentResponse>>> GetByOrganizationIdAsync(Guid organizationId)
        {
            try
            {
                var departments = await _repository.GetAllAsync(
                    filter: d => d.OrganizationId == organizationId,
                    orderBy: q => q.OrderBy(d => d.Name),
                    d => d.Organization,
                    d => d.ProductionLines
                );

                return new ApiResponse<List<DepartmentResponse>>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<List<DepartmentResponse>>(departments),
                    Message = "Departments retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving departments for organization {OrgId}", organizationId);
                return new ApiResponse<List<DepartmentResponse>>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while retrieving departments."
                };
            }
        }

        public async Task<ApiResponse<DepartmentResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var department = await _repository.GetByIdAsync(
                    id,
                    d => d.Organization,
                    d => d.ProductionLines
                );

                if (department == null)
                {
                    return new ApiResponse<DepartmentResponse>
                    {
                        IsSuccessful = false,
                        Message = "Department not found."
                    };
                }

                return new ApiResponse<DepartmentResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<DepartmentResponse>(department),
                    Message = "Department retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving department {Id}", id);
                return new ApiResponse<DepartmentResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while retrieving the department."
                };
            }
        }

        public async Task<ApiResponse<DepartmentResponse>> UpdateAsync(Guid id, UpdateDepartmentRequest request)
        {
            try
            {
                var department = await _repository.GetByIdAsync(id);

                if (department == null)
                {
                    return new ApiResponse<DepartmentResponse>
                    {
                        IsSuccessful = false,
                        Message = "Department not found."
                    };
                }

                // Validate organization exists
                var organizationExists = await _organizationRepository.ExistsAsync(x => x.Id == request.OrganizationId);
                if (!organizationExists)
                {
                    return new ApiResponse<DepartmentResponse>
                    {
                        IsSuccessful = false,
                        Message = "Organization not found."
                    };
                }

                // Check if another department with same code exists in the organization
                var codeExists = await _repository.ExistsAsync(x =>
                    x.Code.ToLower() == request.Code.ToLower() &&
                    x.OrganizationId == request.OrganizationId &&
                    x.Id != id);

                if (codeExists)
                {
                    return new ApiResponse<DepartmentResponse>
                    {
                        IsSuccessful = false,
                        Message = "Another department with this code already exists in the organization."
                    };
                }

                _mapper.Map(request, department);
                _repository.UpdateAsync(department);
                await _repository.SaveChangesAsync();

                // Retrieve updated department with navigation properties
                var updatedDepartment = await _repository.GetByIdAsync(
                    id,
                    d => d.Organization,
                    d => d.ProductionLines
                );

                _logger.LogInformation("Department updated: {Id}", id);

                return new ApiResponse<DepartmentResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<DepartmentResponse>(updatedDepartment),
                    Message = "Department updated successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating department {Id}", id);
                return new ApiResponse<DepartmentResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while updating the department."
                };
            }
        }

        public async Task<ApiResponse<string>> DeleteAsync(Guid id)
        {
            try
            {
                var deleted = await _repository.DeleteAsync(id);

                if (!deleted)
                {
                    return new ApiResponse<string>
                    {
                        IsSuccessful = false,
                        Message = "Department not found."
                    };
                }

                await _repository.SaveChangesAsync();
                _logger.LogInformation("Department deleted: {Id}", id);

                return new ApiResponse<string>
                {
                    IsSuccessful = true,
                    Message = "Department deleted successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting department {Id}", id);
                return new ApiResponse<string>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while deleting the department."
                };
            }
        }
    }
}