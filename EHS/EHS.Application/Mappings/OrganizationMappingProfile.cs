using AutoMapper;
using EHS.Application.DTOs;
using EHS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EHS.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Organization entity and DTOs.
    /// Handles bidirectional mapping between entities and DTOs.
    /// </summary>
    public class OrganizationMappingProfile : Profile
    {
        public OrganizationMappingProfile()
        {
            // Create map: CreateOrganizationRequest -> Organization
            CreateMap<CreateOrganizationRequest, Organization>();

            // Create map: UpdateOrganizationRequest -> Organization
            CreateMap<UpdateOrganizationRequest, Organization>();

            // Create map: Organization -> OrganizationResponse
            CreateMap<Organization, OrganizationResponse>();
        }
    }
}