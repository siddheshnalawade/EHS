using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EHS.API.Controllers.v1
{
    /// <summary>
    /// Controller for Incident Severity master data.
    /// </summary>
    [ApiController]
    [Route("api/v1/incident-severities")]
    [Authorize(Roles = "Admin")]
    public class IncidentSeveritiesController(
        IIncidentSeverityService _incidentSeverityService,
        IValidator<CreateIncidentSeverityRequest> _createValidator,
        IValidator<UpdateIncidentSeverityRequest> _updateValidator) : ControllerBase
    {
        /// <summary>
        /// Creates a new incident severity.
        /// </summary>
        /// <param name="request">Incident severity creation request</param>
        /// <returns>Created incident severity</returns>
        [HttpPost]
        public async Task<IActionResult> CreateIncidentSeverity([FromBody] CreateIncidentSeverityRequest request)
        {
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<IncidentSeverityResponse>
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

            var result = await _incidentSeverityService.CreateAsync(request);

            return result.IsSuccessful
                ? CreatedAtAction(nameof(GetIncidentSeverityById), new { id = result.Data?.Id }, result)
                : BadRequest(result);
        }

        /// <summary>
        /// Retrieves all incident severities ordered by severity level.
        /// </summary>
        /// <returns>List of incident severities</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllIncidentSeverities()
        {
            var result = await _incidentSeverityService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific incident severity by ID.
        /// </summary>
        /// <param name="id">Incident severity ID</param>
        /// <returns>Incident severity details</returns>
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetIncidentSeverityById(Guid id)
        {
            var result = await _incidentSeverityService.GetByIdAsync(id);
            return result.IsSuccessful ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Updates an existing incident severity.
        /// </summary>
        /// <param name="id">Incident severity ID</param>
        /// <param name="request">Update request</param>
        /// <returns>Updated incident severity</returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateIncidentSeverity(Guid id, [FromBody] UpdateIncidentSeverityRequest request)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<IncidentSeverityResponse>
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

            var result = await _incidentSeverityService.UpdateAsync(id, request);
            return result.IsSuccessful ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Deletes an incident severity (soft delete).
        /// </summary>
        /// <param name="id">Incident severity ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteIncidentSeverity(Guid id)
        {
            var result = await _incidentSeverityService.DeleteAsync(id);
            return result.IsSuccessful ? Ok(result) : NotFound(result);
        }
    }
}