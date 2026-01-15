using EHS.Application.Constants;
using EHS.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EHS.Infrastructure.Services
{
    public partial class IncidentService
    {
        public async Task<ApiResponse<IncidentResponse>> AssignToSafetyOfficerAsync(Guid incidentId, Guid safetyOfficerId, Guid userId)
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
                await AddIncidentHistoryAsync(incident.Id, userId, RoleConstant.Admin, IncidentAction.AssignedToSafetyOfficer);
                await _incidentRepository.SaveChangesAsync();

                var updatedIncident = await GetIncidentWithDetailsAsync(incidentId);

                // Send Email to Safety Officer
                if (updatedIncident.AssignedToSafetyOfficer != null && !string.IsNullOrEmpty(updatedIncident.AssignedToSafetyOfficer.Email))
                {
                    await _emailService.SendEmailAsync(new EmailRequest
                    {
                        ToEmail = updatedIncident.AssignedToSafetyOfficer.Email,
                        Subject = $"Incident Assigned: {updatedIncident.Title}",
                        TemplateName = "IncidentAssigned.cshtml",
                        TemplateModel = new
                        {
                            RecipientName = updatedIncident.AssignedToSafetyOfficer.UserName,
                            IncidentId = updatedIncident.Title,
                            Title = updatedIncident.Title,
                            SafetyOfficerName = updatedIncident.AssignedToSafetyOfficer.UserName,
                            ActionUrl = $"http://localhost:4200/incidents/{updatedIncident.Id}"
                        }
                    });
                }

                // Send Email to Initiator
                if (updatedIncident.InitiatedByUser != null && !string.IsNullOrEmpty(updatedIncident.InitiatedByUser.Email))
                {
                    await _emailService.SendEmailAsync(new EmailRequest
                    {
                        ToEmail = updatedIncident.InitiatedByUser.Email,
                        Subject = $"Incident Assigned: {updatedIncident.Title}",
                        TemplateName = "IncidentAssigned.cshtml",
                        TemplateModel = new
                        {
                            RecipientName = updatedIncident.InitiatedByUser.UserName,
                            IncidentId = updatedIncident.Title,
                            Title = updatedIncident.Title,
                            SafetyOfficerName = updatedIncident.AssignedToSafetyOfficer?.UserName ?? "Safety Officer",
                            ActionUrl = $"http://localhost:4200/incidents/{updatedIncident.Id}"
                        }
                    });
                }

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
                await AddIncidentHistoryAsync(incident.Id, userId, RoleConstant.SafetyOfficer, IncidentAction.SafetyOfficerRejected);
                await _incidentRepository.SaveChangesAsync();

                var updatedIncident = await GetIncidentWithDetailsAsync(incidentId);

                // Send Email to Initiator
                if (updatedIncident.InitiatedByUser != null && !string.IsNullOrEmpty(updatedIncident.InitiatedByUser.Email))
                {
                    await _emailService.SendEmailAsync(new EmailRequest
                    {
                        ToEmail = updatedIncident.InitiatedByUser.Email,
                        Subject = $"Incident Rejected: {updatedIncident.Title}",
                        TemplateName = "IncidentRejected.cshtml",
                        TemplateModel = new
                        {
                            RecipientName = updatedIncident.InitiatedByUser.UserName,
                            IncidentId = updatedIncident.Title,
                            Title = updatedIncident.Title,
                            Reason = incident.ReviewerComment,
                            ActionUrl = $"http://localhost:4200/incidents/{updatedIncident.Id}"
                        }
                    });
                }

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
                await AddIncidentHistoryAsync(incident.Id, userId, RoleConstant.SafetyOfficer, IncidentAction.AssignedToInitiator);
                await _incidentRepository.SaveChangesAsync();

                var updatedIncident = await GetIncidentWithDetailsAsync(incidentId);

                // Send Email to Initiator
                if (updatedIncident.InitiatedByUser != null && !string.IsNullOrEmpty(updatedIncident.InitiatedByUser.Email))
                {
                    await _emailService.SendEmailAsync(new EmailRequest
                    {
                        ToEmail = updatedIncident.InitiatedByUser.Email,
                        Subject = $"Incident Reassigned: {updatedIncident.Title}",
                        TemplateName = "IncidentReassigned.cshtml",
                        TemplateModel = new
                        {
                            RecipientName = updatedIncident.InitiatedByUser.UserName,
                            IncidentId = updatedIncident.Id,
                            Title = updatedIncident.Title,
                            Comments = incident.ReviewerComment,
                            ActionUrl = $"http://localhost:4200/incidents/{updatedIncident.Id}"
                        }
                    });
                }

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
                await AddIncidentHistoryAsync(incident.Id, userId, RoleConstant.SafetyOfficer, IncidentAction.AssignedToImplementor);
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
                await AddIncidentHistoryAsync(incident.Id, userId, RoleConstant.SafetyOfficer, IncidentAction.IncidentClosed);
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
    }
}