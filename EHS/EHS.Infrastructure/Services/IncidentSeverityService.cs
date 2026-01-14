using AutoMapper;
using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using EHS.Application.Repositories;
using EHS.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EHS.Infrastructure.Services
{
    public class IncidentSeverityService(
        IRepository<IncidentSeverity> _repository,
        IMapper _mapper,
        ILogger<IncidentSeverityService> _logger) : IIncidentSeverityService
    {
        public async Task<ApiResponse<IncidentSeverityResponse>> CreateAsync(CreateIncidentSeverityRequest request)
        {
            try
            {
                // Check if incident severity with same name already exists
                var nameExists = await _repository.ExistsAsync(x => x.Name.ToLower() == request.Name.ToLower());
                if (nameExists)
                {
                    return new ApiResponse<IncidentSeverityResponse>
                    {
                        IsSuccessful = false,
                        Message = "An incident severity with this name already exists."
                    };
                }

                // Check if severity level already exists
                var levelExists = await _repository.ExistsAsync(x => x.SeverityLevel == request.SeverityLevel);
                if (levelExists)
                {
                    return new ApiResponse<IncidentSeverityResponse>
                    {
                        IsSuccessful = false,
                        Message = "An incident severity with this level already exists."
                    };
                }

                var incidentSeverity = _mapper.Map<IncidentSeverity>(request);
                await _repository.AddAsync(incidentSeverity);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Incident severity created: {Name} (Level: {Level})",
                    incidentSeverity.Name, incidentSeverity.SeverityLevel);

                return new ApiResponse<IncidentSeverityResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<IncidentSeverityResponse>(incidentSeverity),
                    Message = "Incident severity created successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating incident severity");
                return new ApiResponse<IncidentSeverityResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while creating the incident severity."
                };
            }
        }

        public async Task<ApiResponse<List<IncidentSeverityResponse>>> GetAllAsync()
        {
            try
            {
                var incidentSeverities = await _repository.GetAllAsync(
                    orderBy: q => q.OrderBy(x => x.SeverityLevel)
                );

                return new ApiResponse<List<IncidentSeverityResponse>>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<List<IncidentSeverityResponse>>(incidentSeverities),
                    Message = "Incident severities retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving incident severities");
                return new ApiResponse<List<IncidentSeverityResponse>>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while retrieving incident severities."
                };
            }
        }

        public async Task<ApiResponse<IncidentSeverityResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var incidentSeverity = await _repository.GetByIdAsync(id);

                if (incidentSeverity == null)
                {
                    return new ApiResponse<IncidentSeverityResponse>
                    {
                        IsSuccessful = false,
                        Message = "Incident severity not found."
                    };
                }

                return new ApiResponse<IncidentSeverityResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<IncidentSeverityResponse>(incidentSeverity),
                    Message = "Incident severity retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving incident severity {Id}", id);
                return new ApiResponse<IncidentSeverityResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while retrieving the incident severity."
                };
            }
        }

        public async Task<ApiResponse<IncidentSeverityResponse>> UpdateAsync(Guid id, UpdateIncidentSeverityRequest request)
        {
            try
            {
                var incidentSeverity = await _repository.GetByIdAsync(id);

                if (incidentSeverity == null)
                {
                    return new ApiResponse<IncidentSeverityResponse>
                    {
                        IsSuccessful = false,
                        Message = "Incident severity not found."
                    };
                }

                // Check if another incident severity with same name exists
                var nameExists = await _repository.ExistsAsync(x =>
                    x.Name.ToLower() == request.Name.ToLower() && x.Id != id);

                if (nameExists)
                {
                    return new ApiResponse<IncidentSeverityResponse>
                    {
                        IsSuccessful = false,
                        Message = "Another incident severity with this name already exists."
                    };
                }

                // Check if another incident severity with same level exists
                var levelExists = await _repository.ExistsAsync(x =>
                    x.SeverityLevel == request.SeverityLevel && x.Id != id);

                if (levelExists)
                {
                    return new ApiResponse<IncidentSeverityResponse>
                    {
                        IsSuccessful = false,
                        Message = "Another incident severity with this level already exists."
                    };
                }

                _mapper.Map(request, incidentSeverity);
                _repository.UpdateAsync(incidentSeverity);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Incident severity updated: {Id}", id);

                return new ApiResponse<IncidentSeverityResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<IncidentSeverityResponse>(incidentSeverity),
                    Message = "Incident severity updated successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating incident severity {Id}", id);
                return new ApiResponse<IncidentSeverityResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while updating the incident severity."
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
                        Message = "Incident severity not found."
                    };
                }

                await _repository.SaveChangesAsync();
                _logger.LogInformation("Incident severity deleted: {Id}", id);

                return new ApiResponse<string>
                {
                    IsSuccessful = true,
                    Message = "Incident severity deleted successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting incident severity {Id}", id);
                return new ApiResponse<string>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while deleting the incident severity."
                };
            }
        }
    }
}