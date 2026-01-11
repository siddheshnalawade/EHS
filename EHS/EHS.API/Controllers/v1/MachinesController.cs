using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EHS.API.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(Roles = "Admin")]
    public class MachinesController : ControllerBase
    {
        private readonly IMachineService _machineService;
        private readonly IValidator<CreateMachineRequest> _createValidator;
        private readonly IValidator<UpdateMachineRequest> _updateValidator;

        public MachinesController(
            IMachineService machineService,
            IValidator<CreateMachineRequest> createValidator,
            IValidator<UpdateMachineRequest> updateValidator)
        {
            _machineService = machineService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Creates a new machine.
        /// </summary>
        /// <param name="request">Machine creation request</param>
        /// <returns>Created machine</returns>
        [HttpPost]
        public async Task<IActionResult> CreateMachine([FromBody] CreateMachineRequest request)
        {
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<MachineResponse>
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

            var result = await _machineService.CreateAsync(request);

            return result.IsSuccessful
                ? CreatedAtAction(nameof(GetMachineById), new { id = result.Data?.Id }, result)
                : BadRequest(result);
        }

        /// <summary>
        /// Retrieves all machines.
        /// </summary>
        /// <returns>List of machines</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllMachines()
        {
            var result = await _machineService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves machines by production line ID.
        /// </summary>
        /// <param name="productionLineId">Production line ID</param>
        /// <returns>List of machines for the production line</returns>
        [HttpGet("production-line/{productionLineId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMachinesByProductionLine(Guid productionLineId)
        {
            var result = await _machineService.GetByProductionLineIdAsync(productionLineId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific machine by ID.
        /// </summary>
        /// <param name="id">Machine ID</param>
        /// <returns>Machine details</returns>
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMachineById(Guid id)
        {
            var result = await _machineService.GetByIdAsync(id);
            return result.IsSuccessful ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Updates an existing machine.
        /// </summary>
        /// <param name="id">Machine ID</param>
        /// <param name="request">Update request</param>
        /// <returns>Updated machine</returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateMachine(Guid id, [FromBody] UpdateMachineRequest request)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<MachineResponse>
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

            var result = await _machineService.UpdateAsync(id, request);
            return result.IsSuccessful ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Deletes a machine (soft delete).
        /// </summary>
        /// <param name="id">Machine ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteMachine(Guid id)
        {
            var result = await _machineService.DeleteAsync(id);
            return result.IsSuccessful ? Ok(result) : NotFound(result);
        }
    }
}