using AutoMapper;
using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using EHS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EHS.Infrastructure.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserManagementService> _logger;

        public UserManagementService(
            UserManager<ApplicationUser> _userManager,
            RoleManager<ApplicationRole> _roleManager,
            IMapper _mapper,
            ILogger<UserManagementService> _logger)
        {
            this._userManager = _userManager;
            this._roleManager = _roleManager;
            this._mapper = _mapper;
            this._logger = _logger;
        }

        public async Task<ApiResponse<PaginatedResponse<UserResponse>>> GetUsersAsync(int pageNumber, int pageSize, string? searchTerm)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(u => u.FullName.Contains(searchTerm) || u.Email!.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();
            var users = await query
                .OrderBy(u => u.FullName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userResponses = new List<UserResponse>();
            foreach (var user in users)
            {
                var response = _mapper.Map<UserResponse>(user);
                var roles = await _userManager.GetRolesAsync(user);
                response.Roles = roles.ToList();
                userResponses.Add(response);
            }

            return new ApiResponse<PaginatedResponse<UserResponse>>
            {
                IsSuccessful = true,
                Data = new PaginatedResponse<UserResponse>
                {
                    Items = userResponses,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount
                }
            };
        }

        public async Task<ApiResponse<UserResponse>> GetUserByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new ApiResponse<UserResponse> { IsSuccessful = false, Message = "User not found" };
            }

            var response = _mapper.Map<UserResponse>(user);
            var roles = await _userManager.GetRolesAsync(user);
            response.Roles = roles.ToList();

            return new ApiResponse<UserResponse> { IsSuccessful = true, Data = response };
        }

        public async Task<ApiResponse<UserResponse>> AssignRoleAsync(Guid userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new ApiResponse<UserResponse> { IsSuccessful = false, Message = "User not found" };
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return new ApiResponse<UserResponse> { IsSuccessful = false, Message = $"Role '{roleName}' does not exist" };
            }

            if (await _userManager.IsInRoleAsync(user, roleName))
            {
                return new ApiResponse<UserResponse> { IsSuccessful = false, Message = $"User already has role '{roleName}'" };
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                return new ApiResponse<UserResponse>
                {
                    IsSuccessful = false,
                    Message = "Failed to assign role",
                    Errors = result.Errors.Select(e => new ValidationError { Field = "RoleName", Message = e.Description }).ToList()
                };
            }

            _logger.LogInformation("Admin assigned role {RoleName} to user {Email}", roleName, user.Email);
            return await GetUserByIdAsync(userId);
        }

        public async Task<ApiResponse<UserResponse>> RemoveRoleAsync(Guid userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new ApiResponse<UserResponse> { IsSuccessful = false, Message = "User not found" };
            }

            if (roleName == "Admin")
            {
                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                if (admins.Count == 1 && admins[0].Id == userId)
                {
                    return new ApiResponse<UserResponse> { IsSuccessful = false, Message = "Cannot remove the last Admin user" };
                }
            }

            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                return new ApiResponse<UserResponse> { IsSuccessful = false, Message = $"User does not have role '{roleName}'" };
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                return new ApiResponse<UserResponse>
                {
                    IsSuccessful = false,
                    Message = "Failed to remove role",
                    Errors = result.Errors.Select(e => new ValidationError { Field = "RoleName", Message = e.Description }).ToList()
                };
            }

            _logger.LogInformation("Admin removed role {RoleName} from user {Email}", roleName, user.Email);
            return await GetUserByIdAsync(userId);
        }

        public async Task<ApiResponse<UserResponse>> ToggleUserStatusAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new ApiResponse<UserResponse> { IsSuccessful = false, Message = "User not found" };
            }

            if (user.IsActive && await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                var activeAdmins = admins.Where(a => a.IsActive).ToList();
                if (activeAdmins.Count == 1 && activeAdmins[0].Id == userId)
                {
                    return new ApiResponse<UserResponse> { IsSuccessful = false, Message = "Cannot deactivate the last active Admin user" };
                }
            }

            user.IsActive = !user.IsActive;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new ApiResponse<UserResponse>
                {
                    IsSuccessful = false,
                    Message = "Failed to update user status",
                    Errors = result.Errors.Select(e => new ValidationError { Field = "Status", Message = e.Description }).ToList()
                };
            }

            var status = user.IsActive ? "activated" : "deactivated";
            _logger.LogInformation("Admin {Status} user {Email}", status, user.Email);
            return await GetUserByIdAsync(userId);
        }

        public async Task<ApiResponse<List<RoleResponse>>> GetRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var response = _mapper.Map<List<RoleResponse>>(roles);

            return new ApiResponse<List<RoleResponse>> { IsSuccessful = true, Data = response };
        }
    }
}
