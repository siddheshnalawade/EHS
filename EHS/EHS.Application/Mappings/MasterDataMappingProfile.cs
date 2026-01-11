using AutoMapper;
using EHS.Application.DTOs;
using EHS.Domain.Entities;

namespace EHS.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for master data entities.
    /// </summary>
    public class MasterDataMappingProfile : Profile
    {
        public MasterDataMappingProfile()
        {
            // Incident Type mappings
            CreateMap<CreateIncidentTypeRequest, IncidentType>();
            CreateMap<UpdateIncidentTypeRequest, IncidentType>();
            CreateMap<IncidentType, IncidentTypeResponse>();

            // Incident Nature mappings
            CreateMap<CreateIncidentNatureRequest, IncidentNature>();
            CreateMap<UpdateIncidentNatureRequest, IncidentNature>();
            CreateMap<IncidentNature, IncidentNatureResponse>();

            // Incident Severity mappings
            CreateMap<CreateIncidentSeverityRequest, IncidentSeverity>();
            CreateMap<UpdateIncidentSeverityRequest, IncidentSeverity>();
            CreateMap<IncidentSeverity, IncidentSeverityResponse>();

            // Department mappings
            CreateMap<CreateDepartmentRequest, Department>();
            CreateMap<UpdateDepartmentRequest, Department>();
            CreateMap<Department, DepartmentResponse>()
                .ForMember(dest => dest.OrganizationName, opt => opt.MapFrom(src => src.Organization.Name))
                .ForMember(dest => dest.ProductionLineCount, opt => opt.MapFrom(src => src.ProductionLines.Count));

            // Production Line mappings
            CreateMap<CreateProductionLineRequest, ProductionLine>();
            CreateMap<UpdateProductionLineRequest, ProductionLine>();
            CreateMap<ProductionLine, ProductionLineResponse>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.MachineCount, opt => opt.MapFrom(src => src.Machines.Count));

            // Machine mappings
            CreateMap<CreateMachineRequest, Machine>();
            CreateMap<UpdateMachineRequest, Machine>();
            CreateMap<Machine, MachineResponse>()
                .ForMember(dest => dest.ProductionLineName, opt => opt.MapFrom(src => src.ProductionLine.Name));
        }
    }
}