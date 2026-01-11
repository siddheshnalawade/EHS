using EHS.Application.DTOs;
using FluentValidation;

namespace EHS.Application.Validators
{
    public class CreateMachineRequestValidator : AbstractValidator<CreateMachineRequest>
    {
        public CreateMachineRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Machine name is required.")
                .Length(2, 100).WithMessage("Machine name must be between 2 and 100 characters.");

            RuleFor(x => x.EquipmentId)
                .NotEmpty().WithMessage("Equipment ID is required.")
                .Length(2, 50).WithMessage("Equipment ID must be between 2 and 50 characters.")
                .Matches(@"^[A-Z0-9_-]+$").WithMessage("Equipment ID must contain only uppercase letters, numbers, hyphens, and underscores.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.ProductionLineId)
                .NotEmpty().WithMessage("Production line is required.");
        }
    }

    public class UpdateMachineRequestValidator : AbstractValidator<UpdateMachineRequest>
    {
        public UpdateMachineRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Machine name is required.")
                .Length(2, 100).WithMessage("Machine name must be between 2 and 100 characters.");

            RuleFor(x => x.EquipmentId)
                .NotEmpty().WithMessage("Equipment ID is required.")
                .Length(2, 50).WithMessage("Equipment ID must be between 2 and 50 characters.")
                .Matches(@"^[A-Z0-9_-]+$").WithMessage("Equipment ID must contain only uppercase letters, numbers, hyphens, and underscores.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.ProductionLineId)
                .NotEmpty().WithMessage("Production line is required.");
        }
    }
}