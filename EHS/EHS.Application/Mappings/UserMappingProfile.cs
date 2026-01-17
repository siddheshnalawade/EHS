using AutoMapper;
using EHS.Application.DTOs;
using EHS.Domain.Entities;

namespace EHS.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for user and role related entities.
    /// </summary>
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<ApplicationUser, UserResponse>()
                .ForMember(dest => dest.Roles, opt => opt.Ignore()); // Roles are populated manually via UserManager

            CreateMap<ApplicationRole, RoleResponse>();
        }
    }
}
