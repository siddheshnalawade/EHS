using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EHS.API.Controllers.v1
{
    /// <summary>
    /// Controller for Incident Nature master data.
    /// </summary>
    [ApiController]
    [Route("api/v1/incident-natures")]
    [Authorize(Roles = "Admin")]
    public class IncidentNaturesController(
        IIncidentNatureService _incidentNatureService,
        IValidator<CreateIncidentNatureRequest> _createValidator,
        IValidator<UpdateIncidentNatureRequest> _updateValidator) : ControllerBase
    {
        /// <summary>
        /// Creates a new incident nature.
        /// </summary>
        /// <param name="request">Incident nature creation request</param>
        /// <returns>Created incident nature</returns>
        [HttpPost]
        public async Task<IActionResult> CreateIncidentNature([FromBody] CreateIncidentNatureRequest request)
        {
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<IncidentNatureResponse>
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

            var result = await _incidentNatureService.CreateAsync(request);

            return result.IsSuccessful
                ? CreatedAtAction(nameof(GetIncidentNatureById), new { id = result.Data?.Id }, result)
                : BadRequest(result);
        }

        /// <summary>
        /// Retrieves all incident natures.
        /// </summary>
        /// <returns>List of incident natures</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllIncidentNatures()
        {
            var result = await _incidentNatureService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific incident nature by ID.
        /// </summary>
        /// <param name="id">Incident nature ID</param>
        /// <returns>Incident nature details</returns>
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetIncidentNatureById(Guid id)
        {
            var result = await _incidentNatureService.GetByIdAsync(id);
            return result.IsSuccessful ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Updates an existing incident nature.
        /// </summary>
        /// <param name="id">Incident nature ID</param>
        /// <param name="request">Update request</param>
        /// <returns>Updated incident nature</returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateIncidentNature(Guid id, [FromBody] UpdateIncidentNatureRequest request)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<IncidentNatureResponse>
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

            var result = await _incidentNatureService.UpdateAsync(id, request);
            return result.IsSuccessful ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Deletes an incident nature (soft delete).
        /// </summary>
        /// <param name="id">Incident nature ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteIncidentNature(Guid id)
        {
            var result = await _incidentNatureService.DeleteAsync(id);
            return result.IsSuccessful ? Ok(result) : NotFound(result);
        }
    }
}