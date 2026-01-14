using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EHS.API.Controllers.v1
{
    /// <summary>
    /// Controller for incident management operations (Initiator role).
    /// </summary>
    [ApiController]
    [Route("api/v1/incidents")]
    [Authorize]
    public class IncidentsController(
        IIncidentService _incidentService,
        IValidator<CreateIncidentRequest> _createValidator,
        IValidator<UpdateIncidentRequest> _updateValidator) : ControllerBase
    {
        /// <summary>
        /// Creates a new incident report.
        /// </summary>
        /// <param name="request">Incident creation request</param>
        /// <returns>Created incident</returns>
        [HttpPost]
        [Authorize(Roles = "Initiator,Admin")]
        public async Task<IActionResult> CreateIncident([FromBody] CreateIncidentRequest request)
        {
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<IncidentResponse>
                {
                    IsSuccessful = false,
                    Message = "Validation failed",
                    Errors = validationResult.Errors
                        .Select(e => new ValidationError
                        {
                            Field = e.PropertyName,
                            Message = e.ErrorMessage
                        }).ToList()
                });
            }

            var userId = GetCurrentUserId();
            var result = await _incidentService.CreateIncidentAsync(request, userId);

            return result.IsSuccessful
                ? CreatedAtAction(nameof(GetIncidentById), new { id = result.Data?.Id }, result)
                : BadRequest(result);
        }

        /// <summary>
        /// Retrieves all incidents with filtering and pagination.
        /// </summary>
        /// <param name="filter">Filter criteria</param>
        /// <returns>Paginated list of incidents</returns>
        [HttpGet]
        public async Task<IActionResult> GetIncidents([FromQuery] IncidentFilterRequest filter)
        {
            var result = await _incidentService.GetIncidentsAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific incident by ID.
        /// </summary>
        /// <param name="id">Incident ID</param>
        /// <returns>Incident details</returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetIncidentById(Guid id)
        {
            var result = await _incidentService.GetIncidentByIdAsync(id);
            return result.IsSuccessful ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Updates an existing incident (only by initiator, only if in Submitted status).
        /// </summary>
        /// <param name="id">Incident ID</param>
        /// <param name="request">Update request</param>
        /// <returns>Updated incident</returns>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Initiator,Admin")]
        public async Task<IActionResult> UpdateIncident(Guid id, [FromBody] UpdateIncidentRequest request)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<IncidentResponse>
                {
                    IsSuccessful = false,
                    Message = "Validation failed",
                    Errors = validationResult.Errors
                        .Select(e => new ValidationError
                        {
                            Field = e.PropertyName,
                            Message = e.ErrorMessage
                        }).ToList()
                });
            }

            var userId = GetCurrentUserId();
            var result = await _incidentService.UpdateIncidentAsync(id, request, userId);
            return result.IsSuccessful ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Deletes an incident (soft delete, only by initiator, only if in Submitted status).
        /// </summary>
        /// <param name="id">Incident ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Initiator,Admin")]
        public async Task<IActionResult> DeleteIncident(Guid id)
        {
            var userId = GetCurrentUserId();
            var result = await _incidentService.DeleteIncidentAsync(id, userId);
            return result.IsSuccessful ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Retrieves incidents created by the current user.
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated list of user's incidents</returns>
        [HttpGet("my-incidents")]
        [Authorize(Roles = "Initiator,Admin")]
        public async Task<IActionResult> GetMyIncidents(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = GetCurrentUserId();
            var result = await _incidentService.GetMyIncidentsAsync(userId, pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Assigns an incident to a safety officer (Admin only).
        /// </summary>
        /// <param name="id">Incident ID</param>
        /// <param name="request">Assignment request</param>
        /// <returns>Updated incident</returns>
        [HttpPost("{id:guid}/assign-to-safety-officer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignToSafetyOfficer(Guid id, [FromBody] AssignToSafetyOfficerRequest request)
        {
            var result = await _incidentService.AssignToSafetyOfficerAsync(id, request.SafetyOfficerId);
            return result.IsSuccessful ? Ok(result) : BadRequest(result);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim!);
        }
    }
}