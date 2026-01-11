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
    public class IncidentService : IIncidentService
    {
        private readonly IRepository<Incident> _incidentRepository;
        private readonly IRepository<IncidentImplementation> _implementationRepository;
        private readonly IRepository<IncidentStatus> _statusRepository;
        private readonly IRepository<RootCauseAnalysisDetail> _rootCauseRepository;
        private readonly IRepository<ImplementationBenefit> _implementationBenefitRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<IncidentService> _logger;

        public IncidentService(
            IRepository<Incident> incidentRepository,
            IRepository<IncidentImplementation> implementationRepository,
            IRepository<IncidentStatus> statusRepository,
            IRepository<RootCauseAnalysisDetail> rootCauseRepository,
            IRepository<ImplementationBenefit> implementationBenefitRepository,
            IMapper mapper,
            ILogger<IncidentService> logger)
        {
            _incidentRepository = incidentRepository;
            _implementationRepository = implementationRepository;
            _statusRepository = statusRepository;
            _rootCauseRepository = rootCauseRepository;
            _implementationBenefitRepository = implementationBenefitRepository;
            _mapper = mapper;
            _logger = logger;
        }

        #region Initiator Operations

        public async Task<ApiResponse<IncidentResponse>> CreateIncidentAsync(CreateIncidentRequest request, Guid userId)
        {
            try
            {
                // Generate incident number
                var incidentNumber = await GenerateIncidentNumberAsync();

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
                incident.IncidentNumber = incidentNumber;
                incident.InitiatedByUserId = userId;
                incident.IncidentStatusId = submittedStatus.Id;

                await _incidentRepository.AddAsync(incident);
                await _incidentRepository.SaveChangesAsync();

                // Retrieve with navigation properties
                var createdIncident = await GetIncidentWithDetailsAsync(incident.Id);

                _logger.LogInformation("Incident created: {IncidentNumber} by user {UserId}", incidentNumber, userId);

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
                //// Apply filters
                //if (filter.OrganizationId.HasValue)
                //    query = query.Where(i => i.OrganizationId == filter.OrganizationId);

                //if (filter.DepartmentId.HasValue)
                //    query = query.Where(i => i.DepartmentId == filter.DepartmentId);

                //if (filter.IncidentTypeId.HasValue)
                //    query = query.Where(i => i.IncidentTypeId == filter.IncidentTypeId);

                //if (filter.IncidentSeverityId.HasValue)
                //    query = query.Where(i => i.IncidentSeverityId == filter.IncidentSeverityId);

                //if (filter.IncidentStatusId.HasValue)
                //    query = query.Where(i => i.IncidentStatusId == filter.IncidentStatusId);

                //if (filter.StartDate.HasValue)
                //    query = query.Where(i => i.IncidentDate >= filter.StartDate.Value);

                //if (filter.EndDate.HasValue)
                //    query = query.Where(i => i.IncidentDate <= filter.EndDate.Value);

                //if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                //{
                //    var searchTerm = filter.SearchTerm.ToLower();
                //    query = query.Where(i =>
                //        i.IncidentNumber.ToLower().Contains(searchTerm) ||
                //        i.Title.ToLower().Contains(searchTerm) ||
                //        i.Description.ToLower().Contains(searchTerm));
                //}

                //// Include navigation properties
                //query = query
                //    .Include(i => i.IncidentType)
                //    .Include(i => i.IncidentSeverity)
                //    .Include(i => i.IncidentStatus)
                //    .Include(i => i.Department)
                //    .Include(i => i.InitiatedByUser)
                //    .Include(i => i.AssignedToSafetyOfficer)
                //    .Include(i => i.AssignedToImplementor);

                //var totalCount = await query.CountAsync();

                //var incidents = await query
                //    .OrderByDescending(i => i.CreatedAt)
                //    .Skip((filter.PageNumber - 1) * filter.PageSize)
                //    .Take(filter.PageSize)
                //    .ToListAsync();

                var incidents = await _incidentRepository.GetPagedAsync(
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
                );

                return new ApiResponse<PaginatedResponse<IncidentListResponse>>
                {
                    IsSuccessful = true,
                    Data = new PaginatedResponse<IncidentListResponse>
                    {
                        Items = _mapper.Map<List<IncidentListResponse>>(incidents),
                        PageNumber = filter.PageNumber,
                        PageSize = filter.PageSize,
                        TotalCount = 0 // Todo : fix total count
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

        #endregion Initiator Operations

        #region Safety Officer Operations

        public async Task<ApiResponse<IncidentResponse>> AssignToSafetyOfficerAsync(Guid incidentId, Guid safetyOfficerId)
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

                var assignedStatus = await GetStatusByNameAsync(IncidentStatusConstants.Assigned);
                if (assignedStatus == null)
                {
                    return new ApiResponse<IncidentResponse>
                    {
                        IsSuccessful = false,
                        Message = "Incident status 'Assigned' not found in the system."
                    };
                }

                incident.AssignedToSafetyOfficerId = safetyOfficerId;
                incident.AssignedAt = DateTime.UtcNow;
                incident.IncidentStatusId = assignedStatus.Id;

                _incidentRepository.UpdateAsync(incident);
                await _incidentRepository.SaveChangesAsync();

                var updatedIncident = await GetIncidentWithDetailsAsync(incidentId);

                _logger.LogInformation("Incident {Id} assigned to safety officer {OfficerId}", incidentId, safetyOfficerId);

                return new ApiResponse<IncidentResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<IncidentResponse>(updatedIncident),
                    Message = "Incident assigned to safety officer successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning incident {Id} to safety officer", incidentId);
                return new ApiResponse<IncidentResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while assigning the incident."
                };
            }
        }

        public async Task<ApiResponse<IncidentResponse>> RejectIncidentAsync(Guid incidentId, RejectIncidentRequest request, Guid userId)
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

                var rejectedStatus = await GetStatusByNameAsync(IncidentStatusConstants.Rejected);
                if (rejectedStatus == null)
                {
                    return new ApiResponse<IncidentResponse>
                    {
                        IsSuccessful = false,
                        Message = "Incident status 'Rejected' not found in the system."
                    };
                }

                incident.IncidentStatusId = rejectedStatus.Id;
                incident.IsRejected = true;
                incident.RejectedAt = DateTime.UtcNow;
                incident.ReviewerComment = request.Comment;

                _incidentRepository.UpdateAsync(incident);
                await _incidentRepository.SaveChangesAsync();

                var updatedIncident = await GetIncidentWithDetailsAsync(incidentId);

                _logger.LogInformation("Incident {Id} rejected by user {UserId}", incidentId, userId);

                return new ApiResponse<IncidentResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<IncidentResponse>(updatedIncident),
                    Message = "Incident rejected successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting incident {Id}", incidentId);
                return new ApiResponse<IncidentResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while rejecting the incident."
                };
            }
        }

        public async Task<ApiResponse<IncidentResponse>> ReassignToInitiatorAsync(Guid incidentId, ReassignToInitiatorRequest request, Guid userId)
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

                var submittedStatus = await GetStatusByNameAsync(IncidentStatusConstants.Submitted);
                if (submittedStatus == null)
                {
                    return new ApiResponse<IncidentResponse>
                    {
                        IsSuccessful = false,
                        Message = "Incident status 'Submitted' not found in the system."
                    };
                }

                incident.IncidentStatusId = submittedStatus.Id;
                incident.ReviewerComment = request.Comment;

                _incidentRepository.UpdateAsync(incident);
                await _incidentRepository.SaveChangesAsync();

                var updatedIncident = await GetIncidentWithDetailsAsync(incidentId);

                _logger.LogInformation("Incident {Id} reassigned to initiator by user {UserId}", incidentId, userId);

                return new ApiResponse<IncidentResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<IncidentResponse>(updatedIncident),
                    Message = "Incident reassigned to initiator for modifications."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reassigning incident {Id} to initiator", incidentId);
                return new ApiResponse<IncidentResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while reassigning the incident."
                };
            }
        }

        public async Task<ApiResponse<IncidentResponse>> AcceptAndAssignAsync(Guid incidentId, AcceptAndAssignRequest request, Guid userId)
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

                var approvedStatus = await GetStatusByNameAsync(IncidentStatusConstants.Approved);
                if (approvedStatus == null)
                {
                    return new ApiResponse<IncidentResponse>
                    {
                        IsSuccessful = false,
                        Message = "Incident status 'Approved' not found in the system."
                    };
                }

                incident.IncidentStatusId = approvedStatus.Id;
                incident.ApprovedAt = DateTime.UtcNow;
                incident.AssignedToImplementorId = request.ImplementorId;
                incident.ImplementorAssignedAt = DateTime.UtcNow;
                incident.ReviewerComment = request.Comment;

                _incidentRepository.UpdateAsync(incident);
                await _incidentRepository.SaveChangesAsync();

                var updatedIncident = await GetIncidentWithDetailsAsync(incidentId);

                _logger.LogInformation("Incident {Id} approved and assigned to implementor {ImplementorId} by user {UserId}",
                    incidentId, request.ImplementorId, userId);

                return new ApiResponse<IncidentResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<IncidentResponse>(updatedIncident),
                    Message = "Incident approved and assigned to implementor successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting and assigning incident {Id}", incidentId);
                return new ApiResponse<IncidentResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while accepting and assigning the incident."
                };
            }
        }

        public async Task<ApiResponse<PaginatedResponse<IncidentListResponse>>> GetPendingReviewAsync(Guid safetyOfficerId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var assignedStatus = await GetStatusByNameAsync(IncidentStatusConstants.Assigned);

                var (incidents, totalCount) = await _incidentRepository.GetPagedAsync(
                    pageNumber,
                    pageSize,
                    filter: i => i.AssignedToSafetyOfficerId == safetyOfficerId && i.IncidentStatusId == assignedStatus!.Id,
                    orderBy: q => q.OrderBy(i => i.AssignedAt),
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
                    Message = "Pending review incidents retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending review incidents for safety officer {OfficerId}", safetyOfficerId);
                return new ApiResponse<PaginatedResponse<IncidentListResponse>>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while retrieving pending review incidents."
                };
            }
        }

        public async Task<ApiResponse<IncidentResponse>> CloseIncidentAsync(Guid incidentId, CloseIncidentRequest request, Guid userId)
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

                var closedStatus = await GetStatusByNameAsync(IncidentStatusConstants.Closed);
                if (closedStatus == null)
                {
                    return new ApiResponse<IncidentResponse>
                    {
                        IsSuccessful = false,
                        Message = "Incident status 'Closed' not found in the system."
                    };
                }

                incident.IncidentStatusId = closedStatus.Id;
                incident.ClosedAt = DateTime.UtcNow;
                incident.ClosureComment = request.ClosureComment;

                _incidentRepository.UpdateAsync(incident);
                await _incidentRepository.SaveChangesAsync();

                var updatedIncident = await GetIncidentWithDetailsAsync(incidentId);

                _logger.LogInformation("Incident {Id} closed by user {UserId}", incidentId, userId);

                return new ApiResponse<IncidentResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<IncidentResponse>(updatedIncident),
                    Message = "Incident closed successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing incident {Id}", incidentId);
                return new ApiResponse<IncidentResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while closing the incident."
                };
            }
        }

        public async Task<ApiResponse<PaginatedResponse<IncidentListResponse>>> GetPendingVerificationAsync(Guid safetyOfficerId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var verificationStatus = await GetStatusByNameAsync(IncidentStatusConstants.VerificationPending);

                var (incidents, totalCount) = await _incidentRepository.GetPagedAsync(
                    pageNumber,
                    pageSize,
                    filter: i => i.AssignedToSafetyOfficerId == safetyOfficerId && i.IncidentStatusId == verificationStatus!.Id,
                    orderBy: q => q.OrderBy(i => i.CreatedAt),
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
                    Message = "Pending verification incidents retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending verification incidents for safety officer {OfficerId}", safetyOfficerId);
                return new ApiResponse<PaginatedResponse<IncidentListResponse>>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while retrieving pending verification incidents."
                };
            }
        }

        #endregion Safety Officer Operations

        #region Implementor Operations

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

        #endregion Implementor Operations

        #region Helper Methods

        private async Task<string> GenerateIncidentNumberAsync()
        {
            var year = DateTime.UtcNow.Year;
            var lastIncident = await _incidentRepository.GetAllAsync(
                filter: i => i.IncidentNumber.StartsWith($"INC-{year}"),
                orderBy: q => q.OrderByDescending(i => i.CreatedAt)
            );

            var lastNumber = lastIncident.FirstOrDefault()?.IncidentNumber;
            var sequence = 1;

            if (lastNumber != null)
            {
                var parts = lastNumber.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out var num))
                {
                    sequence = num + 1;
                }
            }

            return $"INC-{year}-{sequence:D6}";
        }

        private async Task<IncidentStatus?> GetStatusByNameAsync(string statusName)
        {
            var statuses = await _statusRepository.GetAllAsync(
                filter: s => s.Name == statusName
            );
            return statuses.FirstOrDefault();
        }

        private async Task<Incident?> GetIncidentWithDetailsAsync(Guid id)
        {
            var incidents = await _incidentRepository.GetAllAsync(
                filter: i => i.Id == id,
                orderBy: q => q.OrderByDescending(i => i.CreatedAt),
                i => i.IncidentType,
                i => i.IncidentNature,
                i => i.IncidentSeverity,
                i => i.IncidentStatus,
                i => i.Organization,
                i => i.Department,
                i => i.ProductionLine!,
                i => i.Machine!,
                i => i.InitiatedByUser,
                i => i.AssignedToSafetyOfficer!,
                i => i.AssignedToImplementor!,
                i => i.Implementation!.ClosureAction,
                i => i.Implementation!.ImplementedByUser,
                i => i.Implementation!.Benefits,
                i => i.Implementation!.RootCauseDetails
            );

            return incidents.FirstOrDefault();
        }

        #endregion Helper Methods
    }
}