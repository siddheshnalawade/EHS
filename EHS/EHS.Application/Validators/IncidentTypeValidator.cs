using EHS.Application.DTOs;
using FluentValidation;

namespace EHS.Application.Validators
{
    public class CreateIncidentTypeRequestValidator : AbstractValidator<CreateIncidentTypeRequest>
    {
        public CreateIncidentTypeRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Incident type name is required.")
                .Length(2, 100).WithMessage("Incident type name must be between 2 and 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }

    public class UpdateIncidentTypeRequestValidator : AbstractValidator<UpdateIncidentTypeRequest>
    {
        public UpdateIncidentTypeRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Incident type name is required.")
                .Length(2, 100).WithMessage("Incident type name must be between 2 and 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }
}