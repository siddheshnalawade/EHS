using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EHS.API.Controllers.v1
{
    /// <summary>
    /// Controller for implementor operations.
    /// </summary>
    [ApiController]
    [Route("api/v1/incidents")]
    [Authorize(Roles = "Implementor,Admin")]
    public class ImplementorController(
        IIncidentService _incidentService,
        IValidator<AcceptIncidentRequest> _acceptValidator,
        IValidator<PassToPeerRequest> _passToPeerValidator,
        IValidator<UpdateImplementationRequest> _updateImplementationValidator) : ControllerBase
    {
        /// <summary>
        /// Retrieves incidents assigned to the current implementor.
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated list of assigned incidents</returns>
        [HttpGet("my-implementations")]
        public async Task<IActionResult> GetMyImplementations(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = GetCurrentUserId();
            var result = await _incidentService.GetMyImplementationsAsync(userId, pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Accepts an incident for implementation with estimated timeline.
        /// </summary>
        /// <param name="id">Incident ID</param>
        /// <param name="request">Accept request with estimated days</param>
        /// <returns>Updated incident</returns>
        [HttpPost("{id:guid}/accept")]
        public async Task<IActionResult> AcceptIncident(Guid id, [FromBody] AcceptIncidentRequest request)
        {
            var validationResult = await _acceptValidator.ValidateAsync(request);
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
            var result = await _incidentService.AcceptIncidentAsync(id, request, userId);
            return result.IsSuccessful ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Passes an incident to a peer implementor.
        /// </summary>
        /// <param name="id">Incident ID</param>
        /// <param name="request">Pass to peer request</param>
        /// <returns>Updated incident</returns>
        [HttpPost("{id:guid}/pass-to-peer")]
        public async Task<IActionResult> PassToPeer(Guid id, [FromBody] PassToPeerRequest request)
        {
            var validationResult = await _passToPeerValidator.ValidateAsync(request);
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
            var result = await _incidentService.PassToPeerAsync(id, request, userId);
            return result.IsSuccessful ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Updates implementation details including root cause analysis and corrective actions.
        /// </summary>
        /// <param name="id">Incident ID</param>
        /// <param name="request">Implementation update request</param>
        /// <returns>Updated incident with implementation details</returns>
        [HttpPut("{id:guid}/implementation")]
        public async Task<IActionResult> UpdateImplementation(Guid id, [FromBody] UpdateImplementationRequest request)
        {
            var validationResult = await _updateImplementationValidator.ValidateAsync(request);
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
            var result = await _incidentService.UpdateImplementationAsync(id, request, userId);
            return result.IsSuccessful ? Ok(result) : BadRequest(result);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim!);
        }
    }
}