using EHS.Application.DTOs;
using FluentValidation;

namespace EHS.Application.Validators
{
    public class CreateIncidentSeverityRequestValidator : AbstractValidator<CreateIncidentSeverityRequest>
    {
        public CreateIncidentSeverityRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Incident severity name is required.")
                .Length(2, 100).WithMessage("Incident severity name must be between 2 and 100 characters.");

            RuleFor(x => x.SeverityLevel)
                .GreaterThan(0).WithMessage("Severity level must be greater than 0.")
                .LessThanOrEqualTo(10).WithMessage("Severity level must be 10 or less.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }

    public class UpdateIncidentSeverityRequestValidator : AbstractValidator<UpdateIncidentSeverityRequest>
    {
        public UpdateIncidentSeverityRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Incident severity name is required.")
                .Length(2, 100).WithMessage("Incident severity name must be between 2 and 100 characters.");

            RuleFor(x => x.SeverityLevel)
                .GreaterThan(0).WithMessage("Severity level must be greater than 0.")
                .LessThanOrEqualTo(10).WithMessage("Severity level must be 10 or less.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }
}