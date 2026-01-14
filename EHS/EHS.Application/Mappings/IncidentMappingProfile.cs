using AutoMapper;
using EHS.Application.DTOs;
using EHS.Domain.Entities;

namespace EHS.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for incident-related entities.
    /// </summary>
    public class IncidentMappingProfile : Profile
    {
        public IncidentMappingProfile()
        {
            // Incident mappings
            CreateMap<CreateIncidentRequest, Incident>();

            CreateMap<UpdateIncidentRequest, Incident>()
                .ForMember(dest => dest.OrganizationId, opt => opt.Ignore()); // Cannot change organization

            CreateMap<Incident, IncidentResponse>()
                .ForMember(dest => dest.IncidentTypeName, opt => opt.MapFrom(src => src.IncidentType.Name))
                .ForMember(dest => dest.IncidentNatureName, opt => opt.MapFrom(src => src.IncidentNature.Name))
                .ForMember(dest => dest.IncidentSeverityName, opt => opt.MapFrom(src => src.IncidentSeverity.Name))
                .ForMember(dest => dest.SeverityLevel, opt => opt.MapFrom(src => src.IncidentSeverity.SeverityLevel))
                .ForMember(dest => dest.IncidentStatusName, opt => opt.MapFrom(src => src.IncidentStatus.Name))
                .ForMember(dest => dest.OrganizationName, opt => opt.MapFrom(src => src.Organization.Name))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.ProductionLineName, opt => opt.MapFrom(src => src.ProductionLine != null ? src.ProductionLine.Name : null))
                .ForMember(dest => dest.MachineName, opt => opt.MapFrom(src => src.Machine != null ? src.Machine.Name : null))
                .ForMember(dest => dest.InitiatedByUserName, opt => opt.MapFrom(src => src.InitiatedByUser.UserName))
                .ForMember(dest => dest.AssignedToSafetyOfficerName, opt => opt.MapFrom(src => src.AssignedToSafetyOfficer != null ? src.AssignedToSafetyOfficer.UserName : null))
                .ForMember(dest => dest.AssignedToImplementorName, opt => opt.MapFrom(src => src.AssignedToImplementor != null ? src.AssignedToImplementor.UserName : null));

            CreateMap<Incident, IncidentListResponse>()
                .ForMember(dest => dest.IncidentTypeName, opt => opt.MapFrom(src => src.IncidentType.Name))
                .ForMember(dest => dest.IncidentSeverityName, opt => opt.MapFrom(src => src.IncidentSeverity.Name))
                .ForMember(dest => dest.SeverityLevel, opt => opt.MapFrom(src => src.IncidentSeverity.SeverityLevel))
                .ForMember(dest => dest.IncidentStatusName, opt => opt.MapFrom(src => src.IncidentStatus.Name))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.InitiatedByUserName, opt => opt.MapFrom(src => src.InitiatedByUser.UserName))
                .ForMember(dest => dest.AssignedToSafetyOfficerName, opt => opt.MapFrom(src => src.AssignedToSafetyOfficer != null ? src.AssignedToSafetyOfficer.UserName : null))
                .ForMember(dest => dest.AssignedToImplementorName, opt => opt.MapFrom(src => src.AssignedToImplementor != null ? src.AssignedToImplementor.UserName : null));

            // Implementation mappings
            CreateMap<UpdateImplementationRequest, IncidentImplementation>();

            CreateMap<IncidentImplementation, IncidentImplementationResponse>()
                .ForMember(dest => dest.ClosureActionName, opt => opt.MapFrom(src => src.ClosureAction.Name))
                .ForMember(dest => dest.ImplementedByUserName, opt => opt.MapFrom(src => src.ImplementedByUser.UserName))
                .ForMember(dest => dest.Benefits, opt => opt.MapFrom(src => src.Benefits.Select(b => b.Benefit)))
                .ForMember(dest => dest.RootCauseDetails, opt => opt.MapFrom(src => src.RootCauseDetails));

            // Benefit mapping
            CreateMap<Benefit, BenefitResponse>();

            // Root cause detail mappings
            CreateMap<RootCauseDetailDto, RootCauseAnalysisDetail>();
            CreateMap<RootCauseAnalysisDetail, RootCauseDetailResponse>();
        }
    }
}