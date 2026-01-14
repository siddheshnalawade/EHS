using AutoMapper;
using EHS.Application.Constants;
using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using EHS.Application.Repositories;
using EHS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EHS.Infrastructure.Services
{
    public partial class IncidentService
    {
        public async Task<ApiResponse<IncidentResponse>> CreateIncidentAsync(CreateIncidentRequest request, Guid userId)
        {
            try
            {
                // Get "Submitted" status
                var submittedStatus = await GetStatusByNameAsync(IncidentStatusConstants.Submitted);
                if (submittedStatus == null)
                {
                    return new ApiResponse<IncidentResponse>
                    {
                        IsSuccessful = false,
                        Message = "Incident status 'Submitted' not found in the system."
                    };
                }

                var incident = _mapper.Map<Incident>(request);

                incident.InitiatedByUserId = userId;
                incident.IncidentStatusId = submittedStatus.Id;

                await _incidentRepository.AddAsync(incident);

                await _incidentRepository.SaveChangesAsync();

                // Retrieve with navigation properties
                var createdIncident = await GetIncidentWithDetailsAsync(incident.Id);

                _logger.LogInformation("Incident created by user {UserId}", userId);

                return new ApiResponse<IncidentResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<IncidentResponse>(createdIncident),
                    Message = "Incident created successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating incident");
                return new ApiResponse<IncidentResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while creating the incident."
                };
            }
        }

        public async Task<ApiResponse<PaginatedResponse<IncidentListResponse>>> GetIncidentsAsync(IncidentFilterRequest filter)
        {
            try
            {
                var incidents = (await _incidentRepository.GetPagedAsync(
                    filter.PageNumber,
                    filter.PageSize,
                    filter: null,
                    orderBy: q => q.OrderByDescending(i => i.CreatedAt),
                    i => i.IncidentType,
                    i => i.IncidentSeverity,
                    i => i.IncidentStatus,
                    i => i.Department,
                    i => i.InitiatedByUser,
                    i => i.AssignedToSafetyOfficer,
                    i => i.AssignedToImplementor
                )).ToTuple();

                var incidentsList = incidents.Item1.ToList();

                var mappedIncidents = incidentsList.Select(incident => _mapper.Map<IncidentListResponse>(incident)).ToList();

                return new ApiResponse<PaginatedResponse<IncidentListResponse>>
                {
                    IsSuccessful = true,
                    Data = new PaginatedResponse<IncidentListResponse>
                    {
                        Items = mappedIncidents,
                        PageNumber = filter.PageNumber,
                        PageSize = filter.PageSize,
                        TotalCount = incidents.Item2 // Todo : fix total count
                    },
                    Message = "Incidents retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving incidents");
                return new ApiResponse<PaginatedResponse<IncidentListResponse>>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while retrieving incidents."
                };
            }
        }

        public async Task<ApiResponse<IncidentResponse>> GetIncidentByIdAsync(Guid id)
        {
            try
            {
                var incident = await GetIncidentWithDetailsAsync(id);

                if (incident == null)
                {
                    return new ApiResponse<IncidentResponse>
                    {
                        IsSuccessful = false,
                        Message = "Incident not found."
                    };
                }

                return new ApiResponse<IncidentResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<IncidentResponse>(incident),
                    Message = "Incident retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving incident {Id}", id);
                return new ApiResponse<IncidentResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while retrieving the incident."
                };
            }
        }

        public async Task<ApiResponse<IncidentResponse>> UpdateIncidentAsync(Guid id, UpdateIncidentRequest request, Guid userId)
        {
            try
            {
                var incident = await _incidentRepository.GetByIdAsync(id);

                if (incident == null)
                {
                    return new ApiResponse<IncidentResponse>
                    {
                        IsSuccessful = false,
                        Message = "Incident not found."
                    };
                }

                // Only initiator can update, and only if not yet approved
                if (incident.InitiatedByUserId != userId)
                {
                    return new ApiResponse<IncidentResponse>
                    {
                        IsSuccessful = false,
                        Message = "You can only update incidents you created."
                    };
                }

                var submittedStatus = await GetStatusByNameAsync(IncidentStatusConstants.Submitted);
                if (incident.IncidentStatusId != submittedStatus?.Id)
                {
                    return new ApiResponse<IncidentResponse>
                    {
                        IsSuccessful = false,
                        Message = "You can only update incidents that are in 'Submitted' status."
                    };
                }

                _mapper.Map(request, incident);
                _incidentRepository.UpdateAsync(incident);
                await _incidentRepository.SaveChangesAsync();

                var updatedIncident = await GetIncidentWithDetailsAsync(id);

                _logger.LogInformation("Incident updated: {Id} by user {UserId}", id, userId);

                return new ApiResponse<IncidentResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<IncidentResponse>(updatedIncident),
                    Message = "Incident updated successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating incident {Id}", id);
                return new ApiResponse<IncidentResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while updating the incident."
                };
            }
        }

        public async Task<ApiResponse<string>> DeleteIncidentAsync(Guid id, Guid userId)
        {
            try
            {
                var incident = await _incidentRepository.GetByIdAsync(id);

                if (incident == null)
                {
                    return new ApiResponse<string>
                    {
                        IsSuccessful = false,
                        Message = "Incident not found."
                    };
                }

                // Only initiator can delete, and only if not yet approved
                if (incident.InitiatedByUserId != userId)
                {
                    return new ApiResponse<string>
                    {
                        IsSuccessful = false,
                        Message = "You can only delete incidents you created."
                    };
                }

                var submittedStatus = await GetStatusByNameAsync(IncidentStatusConstants.Submitted);
                if (incident.IncidentStatusId != submittedStatus?.Id)
                {
                    return new ApiResponse<string>
                    {
                        IsSuccessful = false,
                        Message = "You can only delete incidents that are in 'Submitted' status."
                    };
                }

                await _incidentRepository.DeleteAsync(id);
                await _incidentRepository.SaveChangesAsync();

                _logger.LogInformation("Incident deleted: {Id} by user {UserId}", id, userId);

                return new ApiResponse<string>
                {
                    IsSuccessful = true,
                    Message = "Incident deleted successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting incident {Id}", id);
                return new ApiResponse<string>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while deleting the incident."
                };
            }
        }

        public async Task<ApiResponse<PaginatedResponse<IncidentListResponse>>> GetMyIncidentsAsync(Guid userId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var (incidents, totalCount) = await _incidentRepository.GetPagedAsync(
                    pageNumber,
                    pageSize,
                    filter: i => i.InitiatedByUserId == userId,
                    orderBy: q => q.OrderByDescending(i => i.CreatedAt),
                    i => i.IncidentType,
                    i => i.IncidentSeverity,
                    i => i.IncidentStatus,
                    i => i.Department,
                    i => i.InitiatedByUser,
                    i => i.AssignedToSafetyOfficer,
                    i => i.AssignedToImplementor
                );

                return new ApiResponse<PaginatedResponse<IncidentListResponse>>
                {
                    IsSuccessful = true,
                    Data = new PaginatedResponse<IncidentListResponse>
                    {
                        Items = _mapper.Map<List<IncidentListResponse>>(incidents),
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalCount = totalCount
                    },
                    Message = "My incidents retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving incidents for user {UserId}", userId);
                return new ApiResponse<PaginatedResponse<IncidentListResponse>>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while retrieving your incidents."
                };
            }
        }
    }
}