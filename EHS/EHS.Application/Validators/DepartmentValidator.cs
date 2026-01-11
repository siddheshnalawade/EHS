using EHS.Application.DTOs;
using FluentValidation;

namespace EHS.Application.Validators
{
    public class CreateDepartmentRequestValidator : AbstractValidator<CreateDepartmentRequest>
    {
        public CreateDepartmentRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Department name is required.")
                .Length(2, 100).WithMessage("Department name must be between 2 and 100 characters.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Department code is required.")
                .Length(2, 20).WithMessage("Department code must be between 2 and 20 characters.")
                .Matches(@"^[A-Z0-9_-]+$").WithMessage("Department code must contain only uppercase letters, numbers, hyphens, and underscores.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.ManagerName)
                .MaximumLength(100).WithMessage("Manager name cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage("Invalid email format.");

            RuleFor(x => x.OrganizationId)
                .NotEmpty().WithMessage("Organization is required.");
        }
    }

    public class UpdateDepartmentRequestValidator : AbstractValidator<UpdateDepartmentRequest>
    {
        public UpdateDepartmentRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Department name is required.")
                .Length(2, 100).WithMessage("Department name must be between 2 and 100 characters.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Department code is required.")
                .Length(2, 20).WithMessage("Department code must be between 2 and 20 characters.")
                .Matches(@"^[A-Z0-9_-]+$").WithMessage("Department code must contain only uppercase letters, numbers, hyphens, and underscores.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.ManagerName)
                .MaximumLength(100).WithMessage("Manager name cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage("Invalid email format.");

            RuleFor(x => x.OrganizationId)
                .NotEmpty().WithMessage("Organization is required.");
        }
    }
}