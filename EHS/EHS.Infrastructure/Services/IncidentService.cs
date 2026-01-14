using AutoMapper;
using EHS.Application.Interfaces;
using EHS.Application.Repositories;
using EHS.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EHS.Infrastructure.Services
{
    public partial class IncidentService : IIncidentService
    {
        private readonly IRepository<Incident> _incidentRepository;
        private readonly IRepository<IncidentImplementation> _implementationRepository;
        private readonly IRepository<IncidentStatus> _statusRepository;
        private readonly IRepository<RootCauseAnalysisDetail> _rootCauseRepository;
        private readonly IRepository<ImplementationBenefit> _implementationBenefitRepository;
        private readonly IRepository<IncidentComment> _commentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<IncidentService> _logger;

        public IncidentService(
            IRepository<Incident> incidentRepository,
            IRepository<IncidentImplementation> implementationRepository,
            IRepository<IncidentStatus> statusRepository,
            IRepository<RootCauseAnalysisDetail> rootCauseRepository,
            IRepository<ImplementationBenefit> implementationBenefitRepository,
            IRepository<IncidentComment> commentRepository,
            IMapper mapper,
            ILogger<IncidentService> logger)
        {
            _incidentRepository = incidentRepository;
            _implementationRepository = implementationRepository;
            _statusRepository = statusRepository;
            _rootCauseRepository = rootCauseRepository;
            _implementationBenefitRepository = implementationBenefitRepository;
            _commentRepository = commentRepository;
            _mapper = mapper;
            _logger = logger;
        }
    }
}