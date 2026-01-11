using AutoMapper;
using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using EHS.Application.Repositories;
using EHS.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EHS.Infrastructure.Services
{
    public class IncidentTypeService : IIncidentTypeService
    {
        private readonly IRepository<IncidentType> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<IncidentTypeService> _logger;

        public IncidentTypeService(
            IRepository<IncidentType> repository,
            IMapper mapper,
            ILogger<IncidentTypeService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<IncidentTypeResponse>> CreateAsync(CreateIncidentTypeRequest request)
        {
            try
            {
                // Check if incident type with same name already exists
                var exists = await _repository.ExistsAsync(x => x.Name.ToLower() == request.Name.ToLower());
                if (exists)
                {
                    return new ApiResponse<IncidentTypeResponse>
                    {
                        IsSuccessful = false,
                        Message = "An incident type with this name already exists."
                    };
                }

                var incidentType = _mapper.Map<IncidentType>(request);
                await _repository.AddAsync(incidentType);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Incident type created: {Name}", incidentType.Name);

                return new ApiResponse<IncidentTypeResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<IncidentTypeResponse>(incidentType),
                    Message = "Incident type created successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating incident type");
                return new ApiResponse<IncidentTypeResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while creating the incident type."
                };
            }
        }

        public async Task<ApiResponse<List<IncidentTypeResponse>>> GetAllAsync()
        {
            try
            {
                var incidentTypes = await _repository.GetAllAsync(
                    orderBy: q => q.OrderBy(x => x.Name)
                );

                return new ApiResponse<List<IncidentTypeResponse>>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<List<IncidentTypeResponse>>(incidentTypes),
                    Message = "Incident types retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving incident types");
                return new ApiResponse<List<IncidentTypeResponse>>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while retrieving incident types."
                };
            }
        }

        public async Task<ApiResponse<IncidentTypeResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var incidentType = await _repository.GetByIdAsync(id);

                if (incidentType == null)
                {
                    return new ApiResponse<IncidentTypeResponse>
                    {
                        IsSuccessful = false,
                        Message = "Incident type not found."
                    };
                }

                return new ApiResponse<IncidentTypeResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<IncidentTypeResponse>(incidentType),
                    Message = "Incident type retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving incident type {Id}", id);
                return new ApiResponse<IncidentTypeResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while retrieving the incident type."
                };
            }
        }

        public async Task<ApiResponse<IncidentTypeResponse>> UpdateAsync(Guid id, UpdateIncidentTypeRequest request)
        {
            try
            {
                var incidentType = await _repository.GetByIdAsync(id);

                if (incidentType == null)
                {
                    return new ApiResponse<IncidentTypeResponse>
                    {
                        IsSuccessful = false,
                        Message = "Incident type not found."
                    };
                }

                // Check if another incident type with same name exists
                var exists = await _repository.ExistsAsync(x =>
                    x.Name.ToLower() == request.Name.ToLower() && x.Id != id);

                if (exists)
                {
                    return new ApiResponse<IncidentTypeResponse>
                    {
                        IsSuccessful = false,
                        Message = "Another incident type with this name already exists."
                    };
                }

                _mapper.Map(request, incidentType);
                _repository.UpdateAsync(incidentType);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Incident type updated: {Id}", id);

                return new ApiResponse<IncidentTypeResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<IncidentTypeResponse>(incidentType),
                    Message = "Incident type updated successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating incident type {Id}", id);
                return new ApiResponse<IncidentTypeResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while updating the incident type."
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
                        Message = "Incident type not found."
                    };
                }

                await _repository.SaveChangesAsync();
                _logger.LogInformation("Incident type deleted: {Id}", id);

                return new ApiResponse<string>
                {
                    IsSuccessful = true,
                    Message = "Incident type deleted successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting incident type {Id}", id);
                return new ApiResponse<string>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while deleting the incident type."
                };
            }
        }
    }
}