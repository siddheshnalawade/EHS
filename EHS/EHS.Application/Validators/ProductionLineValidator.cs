using EHS.Application.DTOs;
using FluentValidation;

namespace EHS.Application.Validators
{
    public class CreateProductionLineRequestValidator : AbstractValidator<CreateProductionLineRequest>
    {
        public CreateProductionLineRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Production line name is required.")
                .Length(2, 100).WithMessage("Production line name must be between 2 and 100 characters.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Production line code is required.")
                .Length(2, 20).WithMessage("Production line code must be between 2 and 20 characters.")
                .Matches(@"^[A-Z0-9_-]+$").WithMessage("Production line code must contain only uppercase letters, numbers, hyphens, and underscores.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.SupervisorName)
                .MaximumLength(100).WithMessage("Supervisor name cannot exceed 100 characters.");

            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("Department is required.");
        }
    }

    public class UpdateProductionLineRequestValidator : AbstractValidator<UpdateProductionLineRequest>
    {
        public UpdateProductionLineRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Production line name is required.")
                .Length(2, 100).WithMessage("Production line name must be between 2 and 100 characters.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Production line code is required.")
                .Length(2, 20).WithMessage("Production line code must be between 2 and 20 characters.")
                .Matches(@"^[A-Z0-9_-]+$").WithMessage("Production line code must contain only uppercase letters, numbers, hyphens, and underscores.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.SupervisorName)
                .MaximumLength(100).WithMessage("Supervisor name cannot exceed 100 characters.");

            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("Department is required.");
        }
    }
}