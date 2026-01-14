using AutoMapper;
using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using EHS.Application.Repositories;
using EHS.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EHS.Infrastructure.Services
{
    public class MachineService(
        IRepository<Machine> _repository,
        IRepository<ProductionLine> _productionLineRepository,
        IMapper _mapper,
        ILogger<MachineService> _logger) : IMachineService
    {
        public async Task<ApiResponse<MachineResponse>> CreateAsync(CreateMachineRequest request)
        {
            try
            {
                // Validate production line exists
                var productionLineExists = await _productionLineRepository.ExistsAsync(x => x.Id == request.ProductionLineId);
                if (!productionLineExists)
                {
                    return new ApiResponse<MachineResponse>
                    {
                        IsSuccessful = false,
                        Message = "Production line not found."
                    };
                }

                // Check if equipment ID already exists (globally unique)
                var equipmentIdExists = await _repository.ExistsAsync(x =>
                    x.EquipmentId.ToLower() == request.EquipmentId.ToLower());

                if (equipmentIdExists)
                {
                    return new ApiResponse<MachineResponse>
                    {
                        IsSuccessful = false,
                        Message = "A machine with this equipment ID already exists."
                    };
                }

                var machine = _mapper.Map<Machine>(request);
                await _repository.AddAsync(machine);
                await _repository.SaveChangesAsync();

                // Retrieve with navigation properties
                var createdMachine = await _repository.GetByIdAsync(
                    machine.Id,
                    m => m.ProductionLine
                );

                _logger.LogInformation("Machine created: {EquipmentId} in Production Line {LineId}",
                    machine.EquipmentId, machine.ProductionLineId);

                return new ApiResponse<MachineResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<MachineResponse>(createdMachine),
                    Message = "Machine created successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating machine");
                return new ApiResponse<MachineResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while creating the machine."
                };
            }
        }

        public async Task<ApiResponse<List<MachineResponse>>> GetAllAsync()
        {
            try
            {
                var machines = await _repository.GetAllAsync(
                    filter: null,
                    orderBy: q => q.OrderBy(m => m.Name),
                    m => m.ProductionLine
                );

                return new ApiResponse<List<MachineResponse>>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<List<MachineResponse>>(machines),
                    Message = "Machines retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving machines");
                return new ApiResponse<List<MachineResponse>>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while retrieving machines."
                };
            }
        }

        public async Task<ApiResponse<List<MachineResponse>>> GetByProductionLineIdAsync(Guid productionLineId)
        {
            try
            {
                var machines = await _repository.GetAllAsync(
                    filter: m => m.ProductionLineId == productionLineId,
                    orderBy: q => q.OrderBy(m => m.Name),
                    m => m.ProductionLine
                );

                return new ApiResponse<List<MachineResponse>>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<List<MachineResponse>>(machines),
                    Message = "Machines retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving machines for production line {LineId}", productionLineId);
                return new ApiResponse<List<MachineResponse>>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while retrieving machines."
                };
            }
        }

        public async Task<ApiResponse<MachineResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var machine = await _repository.GetByIdAsync(
                    id,
                    m => m.ProductionLine
                );

                if (machine == null)
                {
                    return new ApiResponse<MachineResponse>
                    {
                        IsSuccessful = false,
                        Message = "Machine not found."
                    };
                }

                return new ApiResponse<MachineResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<MachineResponse>(machine),
                    Message = "Machine retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving machine {Id}", id);
                return new ApiResponse<MachineResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while retrieving the machine."
                };
            }
        }

        public async Task<ApiResponse<MachineResponse>> UpdateAsync(Guid id, UpdateMachineRequest request)
        {
            try
            {
                var machine = await _repository.GetByIdAsync(id);

                if (machine == null)
                {
                    return new ApiResponse<MachineResponse>
                    {
                        IsSuccessful = false,
                        Message = "Machine not found."
                    };
                }

                // Validate production line exists
                var productionLineExists = await _productionLineRepository.ExistsAsync(x => x.Id == request.ProductionLineId);
                if (!productionLineExists)
                {
                    return new ApiResponse<MachineResponse>
                    {
                        IsSuccessful = false,
                        Message = "Production line not found."
                    };
                }

                // Check if another machine with same equipment ID exists
                var equipmentIdExists = await _repository.ExistsAsync(x =>
                    x.EquipmentId.ToLower() == request.EquipmentId.ToLower() &&
                    x.Id != id);

                if (equipmentIdExists)
                {
                    return new ApiResponse<MachineResponse>
                    {
                        IsSuccessful = false,
                        Message = "Another machine with this equipment ID already exists."
                    };
                }

                _mapper.Map(request, machine);
                _repository.UpdateAsync(machine);
                await _repository.SaveChangesAsync();

                // Retrieve updated machine with navigation properties
                var updatedMachine = await _repository.GetByIdAsync(
                    id,
                    m => m.ProductionLine
                );

                _logger.LogInformation("Machine updated: {Id}", id);

                return new ApiResponse<MachineResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<MachineResponse>(updatedMachine),
                    Message = "Machine updated successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating machine {Id}", id);
                return new ApiResponse<MachineResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while updating the machine."
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
                        Message = "Machine not found."
                    };
                }

                await _repository.SaveChangesAsync();
                _logger.LogInformation("Machine deleted: {Id}", id);

                return new ApiResponse<string>
                {
                    IsSuccessful = true,
                    Message = "Machine deleted successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting machine {Id}", id);
                return new ApiResponse<string>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while deleting the machine."
                };
            }
        }
    }
}