using AutoMapper;
using EHS.Application.Interfaces;
using EHS.Application.Repositories;
using EHS.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EHS.Infrastructure.Services
{
    public partial class IncidentService(
        IRepository<Incident> _incidentRepository,
        IRepository<IncidentImplementation> _implementationRepository,
        IRepository<IncidentStatus> _statusRepository,
        IRepository<RootCauseAnalysisDetail> _rootCauseRepository,
        IRepository<ImplementationBenefit> _implementationBenefitRepository,
        IRepository<IncidentComment> _commentRepository,
        IMapper _mapper,
        ILogger<IncidentService> _logger) : IIncidentService
    {
    }
}