using EHS.Application.DTOs;
using FluentValidation;

namespace EHS.Application.Validators
{
    public class CreateOrganizationRequestValidator : AbstractValidator<CreateOrganizationRequest>
    {
        public CreateOrganizationRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Organization name is required.")
                .Length(3, 256).WithMessage("Organization name must be between 3 and 256 characters.")
                .Matches(@"^[a-zA-Z0-9\s\-.,&()]*$").WithMessage("Organization name contains invalid characters");

            RuleFor(x => x.LegalName)
               .NotEmpty().WithMessage("Legal name is required")
               .Length(3, 256).WithMessage("Legal name must be between 3 and 256 characters");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required")
                .Length(5, 500).WithMessage("Address must be between 5 and 500 characters");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required")
                .Length(2, 100).WithMessage("City must be between 2 and 100 characters");

            RuleFor(x => x.State)
                .NotEmpty().WithMessage("State is required")
                .Length(2, 100).WithMessage("State must be between 2 and 100 characters");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required")
                .Length(2, 100).WithMessage("Country must be between 2 and 100 characters");

            RuleFor(x => x.PostalCode)
                .NotEmpty().WithMessage("Postal code is required")
                .Matches(@"^\d{5,10}$").WithMessage("Postal code must be 5-10 digits");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^\+?[\d\-\s\(\)]{10,20}$").WithMessage("Phone number format is invalid");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email format is invalid");
        }
    }
}