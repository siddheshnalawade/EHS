using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EHS.API.Controllers
{
    /// <summary>
    /// Controller for safety officer operations.
    /// </summary>
    [ApiController]
    [Route("api/v1/incidents")]
    [Authorize(Roles = "SafetyOfficer,Admin")]
    public class SafetyOfficerController : ControllerBase
    {
        private readonly IIncidentService _incidentService;
        private readonly IValidator<RejectIncidentRequest> _rejectValidator;
        private readonly IValidator<ReassignToInitiatorRequest> _reassignValidator;
        private readonly IValidator<AcceptAndAssignRequest> _acceptAssignValidator;
        private readonly IValidator<CloseIncidentRequest> _closeValidator;

        public SafetyOfficerController(
            IIncidentService incidentService,
            IValidator<RejectIncidentRequest> rejectValidator,
            IValidator<ReassignToInitiatorRequest> reassignValidator,
            IValidator<AcceptAndAssignRequest> acceptAssignValidator,
            IValidator<CloseIncidentRequest> closeValidator)
        {
            _incidentService = incidentService;
            _rejectValidator = rejectValidator;
            _reassignValidator = reassignValidator;
            _acceptAssignValidator = acceptAssignValidator;
            _closeValidator = closeValidator;
        }

        /// <summary>
        /// Retrieves incidents pending review by the current safety officer.
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated list of pending incidents</returns>
        [HttpGet("pending-review")]
        public async Task<IActionResult> GetPendingReview(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = GetCurrentUserId();
            var result = await _incidentService.GetPendingReviewAsync(userId, pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves incidents pending verification by the current safety officer.
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated list of incidents pending verification</returns>
        [HttpGet("pending-verification")]
        public async Task<IActionResult> GetPendingVerification(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = GetCurrentUserId();
            var result = await _incidentService.GetPendingVerificationAsync(userId, pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Rejects an incident with a comment.
        /// </summary>
        /// <param name="id">Incident ID</param>
        /// <param name="request">Rejection request with comment</param>
        /// <returns>Updated incident</returns>
        [HttpPost("{id:guid}/reject")]
        public async Task<IActionResult> RejectIncident(Guid id, [FromBody] RejectIncidentRequest request)
        {
            var validationResult = await _rejectValidator.ValidateAsync(request);
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
            var result = await _incidentService.RejectIncidentAsync(id, request, userId);
            return result.IsSuccessful ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Reassigns an incident to the initiator for modifications.
        /// </summary>
        /// <param name="id">Incident ID</param>
        /// <param name="request">Reassignment request with comment</param>
        /// <returns>Updated incident</returns>
        [HttpPost("{id:guid}/reassign-to-initiator")]
        public async Task<IActionResult> ReassignToInitiator(Guid id, [FromBody] ReassignToInitiatorRequest request)
        {
            var validationResult = await _reassignValidator.ValidateAsync(request);
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
            var result = await _incidentService.ReassignToInitiatorAsync(id, request, userId);
            return result.IsSuccessful ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Accepts an incident and assigns it to an implementor.
        /// </summary>
        /// <param name="id">Incident ID</param>
        /// <param name="request">Accept and assign request</param>
        /// <returns>Updated incident</returns>
        [HttpPost("{id:guid}/accept-and-assign")]
        public async Task<IActionResult> AcceptAndAssign(Guid id, [FromBody] AcceptAndAssignRequest request)
        {
            var validationResult = await _acceptAssignValidator.ValidateAsync(request);
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
            var result = await _incidentService.AcceptAndAssignAsync(id, request, userId);
            return result.IsSuccessful ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Closes an incident after verification.
        /// </summary>
        /// <param name="id">Incident ID</param>
        /// <param name="request">Closure request with comment</param>
        /// <returns>Updated incident</returns>
        [HttpPost("{id:guid}/close")]
        public async Task<IActionResult> CloseIncident(Guid id, [FromBody] CloseIncidentRequest request)
        {
            var validationResult = await _closeValidator.ValidateAsync(request);
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
            var result = await _incidentService.CloseIncidentAsync(id, request, userId);
            return result.IsSuccessful ? Ok(result) : BadRequest(result);
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim!);
        }
    }
}
