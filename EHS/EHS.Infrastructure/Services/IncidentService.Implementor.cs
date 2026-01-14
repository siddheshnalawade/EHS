using EHS.Application.Constants;
using EHS.Application.DTOs;
using EHS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EHS.Infrastructure.Services
{
    public partial class IncidentService
    {
        public async Task<ApiResponse<IncidentResponse>> AcceptIncidentAsync(Guid incidentId, AcceptIncidentRequest request, Guid userId)
        {
            try
            {
                var incident = await _incidentRepository.GetByIdAsync(incidentId);

                if (incident == null)
                {
                    return new ApiResponse<IncidentResponse>
                    {
                        IsSuccessful = false,
                        Message = "Incident not found."
                    };
                }

                if (incident.AssignedToImplementorId != userId)
                {
                    return new ApiResponse<IncidentResponse>
                    {
                        IsSuccessful = false,
                        Message = "This incident is not assigned to you."
                    };
                }

                var inProgressStatus = await GetStatusByNameAsync(IncidentStatusConstants.InProgress);
                if (inProgressStatus == null)
                {
                    return new ApiResponse<IncidentResponse>
                    {
                        IsSuccessful = false,
                        Message = "Incident status 'InProgress' not found in the system."
                    };
                }

                // Create implementation record
                var implementation = new IncidentImplementation
                {
                    IncidentId = incidentId,
                    EstimatedDaysToComplete = request.EstimatedDaysToComplete,
                    AcceptedAt = DateTime.UtcNow,
                    StartedAt = DateTime.UtcNow,
                    ImplementedByUserId = userId,
                    Status = "InProgress"
                };

                await _implementationRepository.AddAsync(implementation);

                incident.IncidentStatusId = inProgressStatus.Id;
                _incidentRepository.UpdateAsync(incident);

                await AddIncidentHistoryAsync(incidentId, userId, RoleConstant.Implementor, IncidentAction.ImplementorAccepted);

                await _implementationRepository.SaveChangesAsync();

                var updatedIncident = await GetIncidentWithDetailsAsync(incidentId);

                _logger.LogInformation("Incident {Id} accepted by implementor {UserId}", incidentId, userId);

                return new ApiResponse<IncidentResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<IncidentResponse>(updatedIncident),
                    Message = "Incident accepted successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting incident {Id}", incidentId);
                return new ApiResponse<IncidentResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while accepting the incident."
                };
            }
        }

        public async Task<ApiResponse<IncidentResponse>> PassToPeerAsync(Guid incidentId, PassToPeerRequest request, Guid userId)
        {
            try
            {
                var incident = await _incidentRepository.GetByIdAsync(incidentId);

                if (incident == null)
                {
                    return new ApiResponse<IncidentResponse>
                    {
                        IsSuccessful = false,
                        Message = "Incident not found."
                    };
                }

                if (incident.AssignedToImplementorId != userId)
                {
                    return new ApiResponse<IncidentResponse>
                    {
                        IsSuccessful = false,
                        Message = "This incident is not assigned to you."
                    };
                }

                incident.AssignedToImplementorId = request.PeerImplementorId;
                incident.ImplementorAssignedAt = DateTime.UtcNow;
                incident.ReviewerComment = request.Comment;

                _incidentRepository.UpdateAsync(incident);
                await _incidentRepository.SaveChangesAsync();

                var updatedIncident = await GetIncidentWithDetailsAsync(incidentId);

                _logger.LogInformation("Incident {Id} passed to peer {PeerId} by user {UserId}",
                    incidentId, request.PeerImplementorId, userId);

                return new ApiResponse<IncidentResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<IncidentResponse>(updatedIncident),
                    Message = "Incident passed to peer successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error passing incident {Id} to peer", incidentId);
                return new ApiResponse<IncidentResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while passing the incident to peer."
                };
            }
        }

        public async Task<ApiResponse<IncidentResponse>> UpdateImplementationAsync(Guid incidentId, UpdateImplementationRequest request, Guid userId)
        {
            try
            {
                var incident = await _incidentRepository.GetByIdAsync(
                    incidentId,
                    i => i.Implementation!
                );

                if (incident == null)
                {
                    return new ApiResponse<IncidentResponse>
                    {
                        IsSuccessful = false,
                        Message = "Incident not found."
                    };
                }

                if (incident.AssignedToImplementorId != userId)
                {
                    return new ApiResponse<IncidentResponse>
                    {
                        IsSuccessful = false,
                        Message = "This incident is not assigned to you."
                    };
                }

                if (incident.Implementation == null)
                {
                    return new ApiResponse<IncidentResponse>
                    {
                        IsSuccessful = false,
                        Message = "Implementation record not found. Please accept the incident first."
                    };
                }

                var verificationStatus = await GetStatusByNameAsync(IncidentStatusConstants.VerificationPending);
                if (verificationStatus == null)
                {
                    return new ApiResponse<IncidentResponse>
                    {
                        IsSuccessful = false,
                        Message = "Incident status 'VerificationPending' not found in the system."
                    };
                }

                // Update implementation
                _mapper.Map(request, incident.Implementation);
                incident.Implementation.CompletedAt = DateTime.UtcNow;
                incident.Implementation.Status = "VerificationPending";

                // Add root cause details
                foreach (var detail in request.RootCauseDetails)
                {
                    var rootCause = _mapper.Map<RootCauseAnalysisDetail>(detail);
                    rootCause.ImplementationId = incident.Implementation.Id;
                    await _rootCauseRepository.AddAsync(rootCause);
                }

                // Add benefits
                foreach (var benefitId in request.BenefitIds)
                {
                    var implementationBenefit = new ImplementationBenefit
                    {
                        ImplementationId = incident.Implementation.Id,
                        BenefitId = benefitId
                    };
                    await _implementationBenefitRepository.AddAsync(implementationBenefit);
                }

                incident.IncidentStatusId = verificationStatus.Id;

                _implementationRepository.UpdateAsync(incident.Implementation);
                _incidentRepository.UpdateAsync(incident);

                if (request.MarkAsCompleted)
                {
                    await AddIncidentHistoryAsync(incidentId, userId, RoleConstant.Implementor, IncidentAction.ImplementationCompleted);
                }

                await _implementationRepository.SaveChangesAsync();

                var updatedIncident = await GetIncidentWithDetailsAsync(incidentId);

                _logger.LogInformation("Implementation updated for incident {Id} by user {UserId}", incidentId, userId);

                return new ApiResponse<IncidentResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<IncidentResponse>(updatedIncident),
                    Message = "Implementation updated successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating implementation for incident {Id}", incidentId);
                return new ApiResponse<IncidentResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while updating the implementation."
                };
            }
        }

        public async Task<ApiResponse<PaginatedResponse<IncidentListResponse>>> GetMyImplementationsAsync(Guid implementorId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var (incidents, totalCount) = await _incidentRepository.GetPagedAsync(
                    pageNumber,
                    pageSize,
                    filter: i => i.AssignedToImplementorId == implementorId,
                    orderBy: q => q.OrderByDescending(i => i.ImplementorAssignedAt),
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
                    Message = "My implementations retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving implementations for implementor {ImplementorId}", implementorId);
                return new ApiResponse<PaginatedResponse<IncidentListResponse>>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while retrieving your implementations."
                };
            }
        }
    }
}