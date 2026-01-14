using AutoMapper;
using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using EHS.Application.Repositories;
using EHS.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EHS.Infrastructure.Services
{
    public class ProductionLineService(
        IRepository<ProductionLine> _repository,
        IRepository<Department> _departmentRepository,
        IMapper _mapper,
        ILogger<ProductionLineService> _logger) : IProductionLineService
    {
        public async Task<ApiResponse<ProductionLineResponse>> CreateAsync(CreateProductionLineRequest request)
        {
            try
            {
                // Validate department exists
                var departmentExists = await _departmentRepository.ExistsAsync(x => x.Id == request.DepartmentId);
                if (!departmentExists)
                {
                    return new ApiResponse<ProductionLineResponse>
                    {
                        IsSuccessful = false,
                        Message = "Department not found."
                    };
                }

                // Check if production line code already exists in this department
                var codeExists = await _repository.ExistsAsync(x =>
                    x.Code.ToLower() == request.Code.ToLower() &&
                    x.DepartmentId == request.DepartmentId);

                if (codeExists)
                {
                    return new ApiResponse<ProductionLineResponse>
                    {
                        IsSuccessful = false,
                        Message = "A production line with this code already exists in the department."
                    };
                }

                var productionLine = _mapper.Map<ProductionLine>(request);
                await _repository.AddAsync(productionLine);
                await _repository.SaveChangesAsync();

                // Retrieve with navigation properties
                var createdProductionLine = await _repository.GetByIdAsync(
                    productionLine.Id,
                    pl => pl.Department,
                    pl => pl.Machines
                );

                _logger.LogInformation("Production line created: {Code} in Department {DeptId}",
                    productionLine.Code, productionLine.DepartmentId);

                return new ApiResponse<ProductionLineResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<ProductionLineResponse>(createdProductionLine),
                    Message = "Production line created successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating production line");
                return new ApiResponse<ProductionLineResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while creating the production line."
                };
            }
        }

        public async Task<ApiResponse<List<ProductionLineResponse>>> GetAllAsync()
        {
            try
            {
                var productionLines = await _repository.GetAllAsync(
                    filter: null,
                    orderBy: q => q.OrderBy(pl => pl.Name),
                    pl => pl.Department,
                    pl => pl.Machines
                );

                return new ApiResponse<List<ProductionLineResponse>>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<List<ProductionLineResponse>>(productionLines),
                    Message = "Production lines retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving production lines");
                return new ApiResponse<List<ProductionLineResponse>>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while retrieving production lines."
                };
            }
        }

        public async Task<ApiResponse<List<ProductionLineResponse>>> GetByDepartmentIdAsync(Guid departmentId)
        {
            try
            {
                var productionLines = await _repository.GetAllAsync(
                    filter: pl => pl.DepartmentId == departmentId,
                    orderBy: q => q.OrderBy(pl => pl.Name),
                    pl => pl.Department,
                    pl => pl.Machines
                );

                return new ApiResponse<List<ProductionLineResponse>>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<List<ProductionLineResponse>>(productionLines),
                    Message = "Production lines retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving production lines for department {DeptId}", departmentId);
                return new ApiResponse<List<ProductionLineResponse>>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while retrieving production lines."
                };
            }
        }

        public async Task<ApiResponse<ProductionLineResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var productionLine = await _repository.GetByIdAsync(
                    id,
                    pl => pl.Department,
                    pl => pl.Machines
                );

                if (productionLine == null)
                {
                    return new ApiResponse<ProductionLineResponse>
                    {
                        IsSuccessful = false,
                        Message = "Production line not found."
                    };
                }

                return new ApiResponse<ProductionLineResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<ProductionLineResponse>(productionLine),
                    Message = "Production line retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving production line {Id}", id);
                return new ApiResponse<ProductionLineResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while retrieving the production line."
                };
            }
        }

        public async Task<ApiResponse<ProductionLineResponse>> UpdateAsync(Guid id, UpdateProductionLineRequest request)
        {
            try
            {
                var productionLine = await _repository.GetByIdAsync(id);

                if (productionLine == null)
                {
                    return new ApiResponse<ProductionLineResponse>
                    {
                        IsSuccessful = false,
                        Message = "Production line not found."
                    };
                }

                // Validate department exists
                var departmentExists = await _departmentRepository.ExistsAsync(x => x.Id == request.DepartmentId);
                if (!departmentExists)
                {
                    return new ApiResponse<ProductionLineResponse>
                    {
                        IsSuccessful = false,
                        Message = "Department not found."
                    };
                }

                // Check if another production line with same code exists in the department
                var codeExists = await _repository.ExistsAsync(x =>
                    x.Code.ToLower() == request.Code.ToLower() &&
                    x.DepartmentId == request.DepartmentId &&
                    x.Id != id);

                if (codeExists)
                {
                    return new ApiResponse<ProductionLineResponse>
                    {
                        IsSuccessful = false,
                        Message = "Another production line with this code already exists in the department."
                    };
                }

                _mapper.Map(request, productionLine);
                _repository.UpdateAsync(productionLine);
                await _repository.SaveChangesAsync();

                // Retrieve updated production line with navigation properties
                var updatedProductionLine = await _repository.GetByIdAsync(
                    id,
                    pl => pl.Department,
                    pl => pl.Machines
                );

                _logger.LogInformation("Production line updated: {Id}", id);

                return new ApiResponse<ProductionLineResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<ProductionLineResponse>(updatedProductionLine),
                    Message = "Production line updated successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating production line {Id}", id);
                return new ApiResponse<ProductionLineResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while updating the production line."
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
                        Message = "Production line not found."
                    };
                }

                await _repository.SaveChangesAsync();
                _logger.LogInformation("Production line deleted: {Id}", id);

                return new ApiResponse<string>
                {
                    IsSuccessful = true,
                    Message = "Production line deleted successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting production line {Id}", id);
                return new ApiResponse<string>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while deleting the production line."
                };
            }
        }
    }
}