using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EHS.API.Controllers.v1
{
    [ApiController]
    [Route("api/v1/incident-types")]
    [Authorize(Roles = "Admin")]
    public class IncidentTypesController(
        IIncidentTypeService _incidentTypeService,
        IValidator<CreateIncidentTypeRequest> _createValidator,
        IValidator<UpdateIncidentTypeRequest> _updateValidator) : ControllerBase
    {
        /// <summary>
        /// Creates a new incident type.
        /// </summary>
        /// <param name="request">Incident type creation request</param>
        /// <returns>Created incident type</returns>
        [HttpPost]
        public async Task<IActionResult> CreateIncidentType([FromBody] CreateIncidentTypeRequest request)
        {
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<IncidentTypeResponse>
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

            var result = await _incidentTypeService.CreateAsync(request);

            return result.IsSuccessful
                ? CreatedAtAction(nameof(GetIncidentTypeById), new { id = result.Data?.Id }, result)
                : BadRequest(result);
        }

        /// <summary>
        /// Retrieves all incident types.
        /// </summary>
        /// <returns>List of incident types</returns>
        [HttpGet]
        [AllowAnonymous] // Allow all authenticated users to view
        public async Task<IActionResult> GetAllIncidentTypes()
        {
            var result = await _incidentTypeService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific incident type by ID.
        /// </summary>
        /// <param name="id">Incident type ID</param>
        /// <returns>Incident type details</returns>
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetIncidentTypeById(Guid id)
        {
            var result = await _incidentTypeService.GetByIdAsync(id);
            return result.IsSuccessful ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Updates an existing incident type.
        /// </summary>
        /// <param name="id">Incident type ID</param>
        /// <param name="request">Update request</param>
        /// <returns>Updated incident type</returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateIncidentType(Guid id, [FromBody] UpdateIncidentTypeRequest request)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<IncidentTypeResponse>
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

            var result = await _incidentTypeService.UpdateAsync(id, request);
            return result.IsSuccessful ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Deletes an incident type (soft delete).
        /// </summary>
        /// <param name="id">Incident type ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteIncidentType(Guid id)
        {
            var result = await _incidentTypeService.DeleteAsync(id);
            return result.IsSuccessful ? Ok(result) : NotFound(result);
        }
    }
}