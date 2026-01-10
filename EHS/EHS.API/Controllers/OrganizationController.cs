using Azure.Core;
using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using EHS.Application.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace EHS.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrganizationController(
        IOrganizationService organizationService,
        IValidator<CreateOrganizationRequest> createValidator,
        IValidator<UpdateOrganizationRequest> updateValidator
        ) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateOrganizaion([FromBody] CreateOrganizationRequest createRequest)
        {
            var validationResult = await createValidator.ValidateAsync(createRequest);
            if (!validationResult.IsValid)
            {
                var response = new ApiResponse<OrganizationResponse>
                {
                    IsSuccessful = false,
                    Message = "Validation failed",
                    Errors = validationResult.Errors
                    .Select(e => new ValidationError
                    {
                        Field = e.PropertyName,
                        Message = e.ErrorMessage
                    }).ToList()
                };

                return BadRequest(response);
            }

            var result = await organizationService.CreateOrganizationAsync(createRequest);

            if (!result.IsSuccessful)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetOrganizationById), new { id = result.Data?.Id }, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrganizations(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null
            )
        {
            // Validate pagination parameters
            if (pageNumber < 1)
            {
                return BadRequest(new ApiResponse<PaginatedResponse<OrganizationResponse>>
                {
                    IsSuccessful = false,
                    Message = "Page number must be greater than 0."
                });
            }

            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest(new ApiResponse<PaginatedResponse<OrganizationResponse>>
                {
                    IsSuccessful = false,
                    Message = "Page size must be between 1 and 100."
                });
            }

            // Call service
            var result = await organizationService.GetAllOrganizationsAsync(pageNumber, pageSize, searchTerm);

            if (!result.IsSuccessful)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrganizationById(Guid id)
        {
            // Call service
            var result = await organizationService.GetOrganizationByIdAsync(id);

            if (!result.IsSuccessful)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateOrganization(
            [FromRoute] Guid id,
            [FromBody] UpdateOrganizationRequest updateRequest
            )
        {
            // Validate request
            var validationResult = await updateValidator.ValidateAsync(updateRequest);
            if (!validationResult.IsValid)
            {
                var response = new ApiResponse<OrganizationResponse>
                {
                    IsSuccessful = false,
                    Message = "Validation failed.",
                    Errors = validationResult.Errors
                        .Select(e => new ValidationError
                        {
                            Field = e.PropertyName,
                            Message = e.ErrorMessage
                        })
                        .ToList()
                };
                return BadRequest(response);
            }

            // Call service
            var result = await organizationService.UpdateOrganizationAsync(id, updateRequest);

            if (!result.IsSuccessful)
            {
                return result.Message == "Organization not found." ? NotFound(result) : BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteOrgnization([FromRoute] Guid id)
        {
            // Call service
            var result = await organizationService.DeleteOrganizationAsync(id);

            if (!result.IsSuccessful)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}