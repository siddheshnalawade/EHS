using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EHS.API.Controllers.v1
{
    /// <summary>
    /// Admin-only controller for user management operations
    /// </summary>
    [ApiController]
    [Route("api/v1/admin/users")]
    [Authorize(Roles = "Admin")]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagementService _userService;
        private readonly IValidator<AssignRoleRequest> _assignRoleValidator;
        private readonly IValidator<RemoveRoleRequest> _removeRoleValidator;

        public UserManagementController(
            IUserManagementService _userService,
            IValidator<AssignRoleRequest> _assignRoleValidator,
            IValidator<RemoveRoleRequest> _removeRoleValidator)
        {
            this._userService = _userService;
            this._assignRoleValidator = _assignRoleValidator;
            this._removeRoleValidator = _removeRoleValidator;
        }

        /// <summary>
        /// Get all users with pagination and optional search
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? searchTerm = null)
        {
            var result = await _userService.GetUsersAsync(pageNumber, pageSize, searchTerm);
            return Ok(result);
        }

        /// <summary>
        /// Get user by ID with roles
        /// </summary>
        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            var result = await _userService.GetUserByIdAsync(userId);
            return result.IsSuccessful ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Assign role to user
        /// </summary>
        [HttpPost("{userId:guid}/assign-role")]
        public async Task<IActionResult> AssignRole(
            Guid userId,
            [FromBody] AssignRoleRequest request)
        {
            var validationResult = await _assignRoleValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<UserResponse>
                {
                    IsSuccessful = false,
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => new ValidationError { Field = e.PropertyName, Message = e.ErrorMessage }).ToList()
                });
            }

            var result = await _userService.AssignRoleAsync(userId, request.RoleName);
            return result.IsSuccessful ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Remove role from user
        /// </summary>
        [HttpPost("{userId:guid}/remove-role")]
        public async Task<IActionResult> RemoveRole(
            Guid userId,
            [FromBody] RemoveRoleRequest request)
        {
            var validationResult = await _removeRoleValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<UserResponse>
                {
                    IsSuccessful = false,
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => new ValidationError { Field = e.PropertyName, Message = e.ErrorMessage }).ToList()
                });
            }

            var result = await _userService.RemoveRoleAsync(userId, request.RoleName);
            return result.IsSuccessful ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Activate or deactivate user
        /// </summary>
        [HttpPost("{userId:guid}/toggle-status")]
        public async Task<IActionResult> ToggleUserStatus(Guid userId)
        {
            var result = await _userService.ToggleUserStatusAsync(userId);
            return result.IsSuccessful ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get all available roles
        /// </summary>
        [HttpGet("~/api/v1/admin/roles")]
        public async Task<IActionResult> GetRoles()
        {
            var result = await _userService.GetRolesAsync();
            return Ok(result);
        }
    }
}
