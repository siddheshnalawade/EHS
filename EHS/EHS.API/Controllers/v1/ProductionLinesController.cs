using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EHS.API.Controllers.v1
{
    [ApiController]
    [Route("api/v1/production-lines")]
    [Authorize(Roles = "Admin")]
    public class ProductionLinesController(
        IProductionLineService _productionLineService,
        IValidator<CreateProductionLineRequest> _createValidator,
        IValidator<UpdateProductionLineRequest> _updateValidator) : ControllerBase
    {
        /// <summary>
        /// Creates a new production line.
        /// </summary>
        /// <param name="request">Production line creation request</param>
        /// <returns>Created production line</returns>
        [HttpPost]
        public async Task<IActionResult> CreateProductionLine([FromBody] CreateProductionLineRequest request)
        {
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<ProductionLineResponse>
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

            var result = await _productionLineService.CreateAsync(request);

            return result.IsSuccessful
                ? CreatedAtAction(nameof(GetProductionLineById), new { id = result.Data?.Id }, result)
                : BadRequest(result);
        }

        /// <summary>
        /// Retrieves all production lines.
        /// </summary>
        /// <returns>List of production lines</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProductionLines()
        {
            var result = await _productionLineService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves production lines by department ID.
        /// </summary>
        /// <param name="departmentId">Department ID</param>
        /// <returns>List of production lines for the department</returns>
        [HttpGet("department/{departmentId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductionLinesByDepartment(Guid departmentId)
        {
            var result = await _productionLineService.GetByDepartmentIdAsync(departmentId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific production line by ID.
        /// </summary>
        /// <param name="id">Production line ID</param>
        /// <returns>Production line details</returns>
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductionLineById(Guid id)
        {
            var result = await _productionLineService.GetByIdAsync(id);
            return result.IsSuccessful ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Updates an existing production line.
        /// </summary>
        /// <param name="id">Production line ID</param>
        /// <param name="request">Update request</param>
        /// <returns>Updated production line</returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateProductionLine(Guid id, [FromBody] UpdateProductionLineRequest request)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<ProductionLineResponse>
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

            var result = await _productionLineService.UpdateAsync(id, request);
            return result.IsSuccessful ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Deletes a production line (soft delete).
        /// </summary>
        /// <param name="id">Production line ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteProductionLine(Guid id)
        {
            var result = await _productionLineService.DeleteAsync(id);
            return result.IsSuccessful ? Ok(result) : NotFound(result);
        }
    }
}