using EHS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EHS.Infrastructure.Services
{
    public partial class IncidentService
    {
        private async Task AddIncidentHistoryAsync(Guid incidentId, Guid userId, string role, string action, string? note = null)
        {
            var content = action;
            if (!string.IsNullOrWhiteSpace(note))
            {
                content += $". Note: {note}";
            }

            var comment = new IncidentComment
            {
                IncidentId = incidentId,
                CommentedByUserId = userId,
                CommentedAsRole = role,
                Content = content,
                IsInternal = true, // History logs are internal/system comments
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _commentRepository.AddAsync(comment);
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
    }
}