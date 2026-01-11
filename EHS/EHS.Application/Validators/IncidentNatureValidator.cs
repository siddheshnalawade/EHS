using EHS.Application.DTOs;
using FluentValidation;

namespace EHS.Application.Validators
{
    public class CreateIncidentNatureRequestValidator : AbstractValidator<CreateIncidentNatureRequest>
    {
        public CreateIncidentNatureRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Incident nature name is required.")
                .Length(2, 100).WithMessage("Incident nature name must be between 2 and 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }

    public class UpdateIncidentNatureRequestValidator : AbstractValidator<UpdateIncidentNatureRequest>
    {
        public UpdateIncidentNatureRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Incident nature name is required.")
                .Length(2, 100).WithMessage("Incident nature name must be between 2 and 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }
}